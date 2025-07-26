namespace LibraryManagement.Services.Core.Tests.Services.MembershipServiceTests
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using LibraryManagement.Web.ViewModels.Membership;
    using Microsoft.AspNetCore.Identity;
    using Moq;
    using System.Linq.Expressions;

    [TestFixture]
    public class MembershipServiceTests
    {
        private Mock<IMembershipRepository> _membershipRepositoryMock;
        private Mock<UserManager<IdentityUser>> _userManagerMock;
        private MembershipService _membershipService;

        [SetUp]
        public void SetUp()
        {
            _membershipRepositoryMock = new Mock<IMembershipRepository>();

            var store = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!
            );

            _membershipService = new MembershipService(_membershipRepositoryMock.Object, _userManagerMock.Object);
        }

        [Test]
        public void ApplyForMembershipAsync_UserNotFound_ThrowsException()
        {

            string userId = Guid.NewGuid().ToString();
            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync((IdentityUser?)null);

            var inputModel = new MemberApplicationInputModel { Name = "Test", Reason = "Test reason" };

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _membershipService.ApplyForMembershipAsync(userId, inputModel));

            Assert.That(ex?.Message, Is.EqualTo("User not found"));
        }

        [TestCase(MembershipStatus.Pending)]
        [TestCase(MembershipStatus.Approved)]
        public void ApplyForMembershipAsync_ExistingApprovedOrPending_ThrowsException(MembershipStatus status)
        {

            string userId = Guid.NewGuid().ToString();

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(new IdentityUser { Id = userId });

            _membershipRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Member, bool>>>()))
                .ReturnsAsync(new Member { UserId = userId, Status = status });

            var inputModel = new MemberApplicationInputModel { Name = "Test", Reason = "Test reason" };

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _membershipService.ApplyForMembershipAsync(userId, inputModel));

            Assert.That(ex?.Message, Is.EqualTo("You have already applied for or have been granted membership."));
        }

        [Test]
        public async Task ApplyForMembershipAsync_ExistingRejected_UpdatesMembership()
        {

            string userId = Guid.NewGuid().ToString();

            var existingMember = new Member
            {
                UserId = userId,
                Status = MembershipStatus.Rejected
            };

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(new IdentityUser { Id = userId });

            _membershipRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Member, bool>>>()))
                .ReturnsAsync(existingMember);

            var inputModel = new MemberApplicationInputModel { Name = "Updated Name", Reason = "Updated Reason" };

            await _membershipService.ApplyForMembershipAsync(userId, inputModel);

            _membershipRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Member>(m =>
                m.Name == inputModel.Name &&
                m.MembershipApplicationReason == inputModel.Reason &&
                m.Status == MembershipStatus.Pending
            )), Times.Once);
        }

        [Test]
        public async Task ApplyForMembershipAsync_NoExistingMembership_AddsNewMembership()
        {

            string userId = Guid.NewGuid().ToString();
            var user = new IdentityUser { Id = userId };

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _membershipRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Member, bool>>>()))
                .ReturnsAsync((Member?)null);

            var inputModel = new MemberApplicationInputModel { Name = "New User", Reason = "Reason" };

            await _membershipService.ApplyForMembershipAsync(userId, inputModel);

            _membershipRepositoryMock.Verify(r => r.AddAsync(It.Is<Member>(m =>
                m.Name == inputModel.Name &&
                m.MembershipApplicationReason == inputModel.Reason &&
                m.Status == MembershipStatus.Pending &&
                m.UserId == userId
            )), Times.Once);

            _membershipRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetPendingApplications_ReturnsMappedViewModels()
        {

            var pendingMembers = new List<Member>
            {
                new Member
                {
                    Id = Guid.NewGuid(),
                    Name = "Alice",
                    JoinDate = new DateTime(2024, 5, 1),
                    MembershipApplicationReason = "Love books",
                    Status = MembershipStatus.Pending,
                    User = new IdentityUser { Email = "alice@example.com" }
                },
                new Member
                {
                    Id = Guid.NewGuid(),
                    Name = "Bob",
                    JoinDate = new DateTime(2024, 5, 2),
                    MembershipApplicationReason = "Wants access",
                    Status = MembershipStatus.Pending,
                    User = null 
                }
            };

            _membershipRepositoryMock
                .Setup(r => r.GetPendingApplicationsAsync())
                .ReturnsAsync(pendingMembers);

            var result = await _membershipService.GetPendingApplications();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(2).Items);

            var first = result.First();
            Assert.That(first.Name, Is.EqualTo("Alice"));
            Assert.That(first.Email, Is.EqualTo("alice@example.com"));

            var second = result.Skip(1).First();
            Assert.That(second.Email, Is.EqualTo("Unknown Email"));
        }

        [Test]
        public async Task GetPendingApplications_EmptyList_ReturnsEmpty()
        {

            _membershipRepositoryMock
                .Setup(r => r.GetPendingApplicationsAsync())
                .ReturnsAsync(new List<Member>());

            var result = await _membershipService.GetPendingApplications();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetMembershipByUserIdAsync_MembershipExists_ReturnsMember()
        {

            var userId = "test-user-id";
            var member = new Member
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = "Test Member"
            };

            _membershipRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Member, bool>>>()))
                .ReturnsAsync(member);

            var result = await _membershipService.GetMembershipByUserIdAsync(userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.UserId, Is.EqualTo(userId));
        }

        [Test]
        public async Task GetMembershipByUserIdAsync_MembershipDoesNotExist_ReturnsNull()
        {

            var userId = "nonexistent-user";

            _membershipRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Member, bool>>>()))
                .ReturnsAsync((Member?)null);

            var result = await _membershipService.GetMembershipByUserIdAsync(userId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateMembershipStatusAsync_ValidMember_UpdatesStatusAndReturnsTrue()
        {

            var memberId = Guid.NewGuid();
            var member = new Member
            {
                Id = memberId,
                Status = MembershipStatus.Pending
            };

            _membershipRepositoryMock
                .Setup(r => r.GetByIdAsync(memberId))
                .ReturnsAsync(member);

            _membershipRepositoryMock
                .Setup(r => r.UpdateAsync(member))
                .ReturnsAsync(true);

            var result = await _membershipService.UpdateMembershipStatusAsync(memberId, MembershipStatus.Approved);

            Assert.That(result, Is.True);
            Assert.That(member.Status, Is.EqualTo(MembershipStatus.Approved));
        }

        [Test]
        public async Task UpdateMembershipStatusAsync_MemberNotFound_ReturnsFalse()
        {

            var memberId = Guid.NewGuid();

            _membershipRepositoryMock
                .Setup(r => r.GetByIdAsync(memberId))
                .ReturnsAsync((Member?)null);

            var result = await _membershipService.UpdateMembershipStatusAsync(memberId, MembershipStatus.Approved);

            Assert.That(result, Is.False);
            _membershipRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Member>()), Times.Never);
        }

        [Test]
        public async Task GetApprovedMembersAsync_WithApprovedMembers_ReturnsMappedViewModels()
        {

            var members = new List<Member>
            {
                new Member { Id = Guid.NewGuid(), Name = "John Doe", JoinDate = new DateTime(2024, 1, 15) },
                new Member { Id = Guid.NewGuid(), Name = "Jane Smith", JoinDate = new DateTime(2024, 2, 20) }
            };

            _membershipRepositoryMock
                .Setup(r => r.GetApprovedMembersAsync())
                .ReturnsAsync(members);

            var result = await _membershipService.GetApprovedMembersAsync();

            Assert.That(result, Is.Not.Null);
            var list = result as List<ApprovedMemberViewModel> ?? new List<ApprovedMemberViewModel>(result);
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list[0].Name, Is.EqualTo("John Doe"));
            Assert.That(list[1].JoinDate, Is.EqualTo(new DateTime(2024, 2, 20)));
        }

        [Test]
        public async Task GetApprovedMembersAsync_WhenNoApprovedMembers_ReturnsEmptyList()
        {

            _membershipRepositoryMock
                .Setup(r => r.GetApprovedMembersAsync())
                .ReturnsAsync(new List<Member>());

            var result = await _membershipService.GetApprovedMembersAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }
    }
}
