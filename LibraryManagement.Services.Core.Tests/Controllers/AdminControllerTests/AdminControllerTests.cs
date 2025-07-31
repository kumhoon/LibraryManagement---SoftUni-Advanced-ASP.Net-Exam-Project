namespace LibraryManagement.Services.Core.Tests.Controllers.AdminControllerTests
{
    using static LibraryManagement.GCommon.Messages.AdminMessages;
    using static LibraryManagement.GCommon.ErrorMessages;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.Areas.Admin.Controllers;
    using LibraryManagement.Web.ViewModels.Membership;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.Logging;
    using Moq;
    using LibraryManagement.Data.Models;

    [TestFixture]
    public class AdminControllerTests
    {
        private Mock<IMembershipService> _membershipServiceMock;
        private Mock<ILogger<AdminController>> _loggerMock;
        private AdminController _adminController;

        [SetUp]
        public void Setup() 
        {
            _membershipServiceMock = new Mock<IMembershipService>();
            _loggerMock = new Mock<ILogger<AdminController>>();
            _adminController = new AdminController(_membershipServiceMock.Object, _loggerMock.Object);
            _adminController.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        [TearDown]
        public void TearDown() => _adminController.Dispose();

        [Test]
        public async Task Members_WhenServiceReturnsMembers_ShouldReturnViewWithModel()
        {

            var expectedMembers = new List<ApprovedMemberViewModel>
            {
                new ApprovedMemberViewModel { Name = "John Doe" },
                new ApprovedMemberViewModel { Name = "Jane Smith" }
            };

            _membershipServiceMock
                .Setup(s => s.GetApprovedMembersAsync())
                .ReturnsAsync(expectedMembers);

            var result = await _adminController.Members();

            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;

            Assert.That(viewResult.Model, Is.AssignableTo<IEnumerable<ApprovedMemberViewModel>>());
            var model = (IEnumerable<ApprovedMemberViewModel>)viewResult.Model;

            Assert.That(model.Count(), Is.EqualTo(2));
            Assert.That(model.First().Name, Is.EqualTo("John Doe"));
        }

        [Test]
        public async Task Members_WhenServiceThrowsException_ShouldRedirectAndSetErrorMessage()
        {

            _membershipServiceMock
                .Setup(s => s.GetApprovedMembersAsync())
                .ThrowsAsync(new Exception());

            IActionResult result = await _adminController.Members();

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            RedirectToActionResult redirectResult = (RedirectToActionResult)result;

            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(AdminController.Dashboard)));
            Assert.That(_adminController.TempData.ContainsKey("ErrorMessage"), Is.True);
            Assert.That(_adminController.TempData["ErrorMessage"], Is.EqualTo(UnexpectedErrorMessage));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(UnexpectedErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task ReviewApplications_WhenServiceReturnsPendingMembers_ShouldReturnViewWithModel()
        {
            var expectedPendingMembers = new List<MembershipPendingViewModel>
            {
                new MembershipPendingViewModel { Name = "Alice Johnson" },
                new MembershipPendingViewModel { Name = "Bob Brown" }
            };

            _membershipServiceMock
                .Setup(s => s.GetPendingApplications())
                .ReturnsAsync(expectedPendingMembers);

            var result = await _adminController.ReviewApplications();
            Assert.IsInstanceOf<ViewResult>(result);

            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.AssignableTo<IEnumerable<MembershipPendingViewModel>>());

            var model = (IEnumerable<MembershipPendingViewModel>)viewResult.Model;
            Assert.That(model.Count(), Is.EqualTo(2));
            Assert.That(model.First().Name, Is.EqualTo("Alice Johnson"));
        }

        [Test]
        public async Task ReviewApplications_WhenServiceThrowsException_ShouldRedirectAndSetErrorMessage()
        {
            _membershipServiceMock
                .Setup(s => s.GetPendingApplications())
                .ThrowsAsync(new Exception());

            IActionResult result = await _adminController.ReviewApplications();
            Assert.IsInstanceOf<RedirectToActionResult>(result);

            RedirectToActionResult redirectResult = (RedirectToActionResult)result;
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(AdminController.Dashboard)));

            Assert.That(_adminController.TempData.ContainsKey("ErrorMessage"), Is.True);
            Assert.That(_adminController.TempData["ErrorMessage"], Is.EqualTo(UnexpectedErrorMessage));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(UnexpectedErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task ApproveMembership_WhenServiceSucceeds_ShouldRedirectWithSuccessMessage() 
        {
            var memberId = Guid.NewGuid();
            _membershipServiceMock
                .Setup(s => s.UpdateMembershipStatusAsync(memberId, MembershipStatus.Approved))
                .ReturnsAsync(true);

            var result = await _adminController.ApproveMembership(memberId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;

            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(AdminController.ReviewApplications)));
            Assert.That(_adminController.TempData.ContainsKey("SuccessMessage"), Is.True);
            Assert.That(_adminController.TempData["SuccessMessage"], Is.EqualTo(MembershipApproved));
        }

        [Test]
        public async Task ApproveMembership_WhenServiceFails_ShouldRedirectWithErrorMessage() 
        {
            var memberId = Guid.NewGuid();

            _membershipServiceMock
                .Setup(s => s.UpdateMembershipStatusAsync(memberId, MembershipStatus.Approved))
                .ReturnsAsync(false);

            var result = await _adminController.ApproveMembership(memberId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;

            Assert.That(redirect.ActionName, Is.EqualTo(nameof(AdminController.ReviewApplications)));
            Assert.That(_adminController.TempData.ContainsKey("ErrorMessage"), Is.True);
            Assert.That(_adminController.TempData["ErrorMessage"], Is.EqualTo(MembershipApprovedFailed));
        }

        [Test]
        public async Task ApproveMembership_WhenExceptionThrown_ShouldRedirectWithUnexpectedErrorMessageAndLogError()
        {
            var memberId = Guid.NewGuid();

            _membershipServiceMock
                .Setup(s => s.UpdateMembershipStatusAsync(memberId, MembershipStatus.Approved))
                .ThrowsAsync(new Exception());

            var result = await _adminController.ApproveMembership(memberId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;

            Assert.That(redirect.ActionName, Is.EqualTo(nameof(AdminController.ReviewApplications)));
            Assert.That(_adminController.TempData.ContainsKey("ErrorMessage"), Is.True);
            Assert.That(_adminController.TempData["ErrorMessage"], Is.EqualTo(UnexpectedErrorMessage));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(ApproveMembershipErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task RejectMembership_WhenServiceSucceeds_ShouldRedirectWithSuccessMessage() 
        {
            var memberId = Guid.NewGuid();
            _membershipServiceMock
                .Setup(s => s.UpdateMembershipStatusAsync(memberId, MembershipStatus.Rejected))
                .ReturnsAsync(true);

            var result = await _adminController.RejectMembership(memberId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;

            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(AdminController.ReviewApplications)));
            Assert.That(_adminController.TempData.ContainsKey("SuccessMessage"), Is.True);
            Assert.That(_adminController.TempData["SuccessMessage"], Is.EqualTo(MembershipRejected));
        }

        [Test]
        public async Task RejectMembership_WhenServiceFails_ShouldRedirectWithErrorMessage() 
        {
            var memberId = Guid.NewGuid();
            _membershipServiceMock
                .Setup(s => s.UpdateMembershipStatusAsync(memberId, MembershipStatus.Rejected))
                .ReturnsAsync(false);

            var result = await _adminController.RejectMembership(memberId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;

            Assert.That(redirect.ActionName, Is.EqualTo(nameof(AdminController.ReviewApplications)));
            Assert.That(_adminController.TempData.ContainsKey("ErrorMessage"), Is.True);
            Assert.That(_adminController.TempData["ErrorMessage"], Is.EqualTo(MembershipRejectedFailed));
        }

        [Test]
        public async Task RejectMembership_WhenExceptionThrown_ShouldRedirectWithUnexpectedErrorMessageAndLogError()
        {
            var memberId = Guid.NewGuid();
            _membershipServiceMock
                .Setup(s => s.UpdateMembershipStatusAsync(memberId, MembershipStatus.Rejected))
                .ThrowsAsync(new Exception());

            var result = await _adminController.RejectMembership(memberId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;

            Assert.That(redirect.ActionName, Is.EqualTo(nameof(AdminController.ReviewApplications)));
            Assert.That(_adminController.TempData.ContainsKey("ErrorMessage"), Is.True);
            Assert.That(_adminController.TempData["ErrorMessage"], Is.EqualTo(UnexpectedErrorMessage));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(RejectMembershipErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task RevokeMembership_WhenServiceSucceeds_ShouldRedirectWithSuccessMessage() 
        {
            var memberId = Guid.NewGuid();
            _membershipServiceMock
                .Setup(s => s.UpdateMembershipStatusAsync(memberId, MembershipStatus.Revoked))
                .ReturnsAsync(true);

            var result = await _adminController.RevokeMembership(memberId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;

            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(AdminController.Members)));
            Assert.That(_adminController.TempData.ContainsKey("SuccessMessage"), Is.True);
            Assert.That(_adminController.TempData["SuccessMessage"], Is.EqualTo(MembershipRevoked));
        }

        [Test]
        public async Task RevokeMembership_WhenServiceFails_ShouldRedirectWithErrorMessage() 
        {
            var memberId = Guid.NewGuid();
            _membershipServiceMock
                .Setup(s => s.UpdateMembershipStatusAsync(memberId, MembershipStatus.Revoked))
                .ReturnsAsync(false);

            var result = await _adminController.RevokeMembership(memberId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;

            Assert.That(redirect.ActionName, Is.EqualTo(nameof(AdminController.Members)));
            Assert.That(_adminController.TempData.ContainsKey("ErrorMessage"), Is.True);
            Assert.That(_adminController.TempData["ErrorMessage"], Is.EqualTo(MembershipRevokedFailed));
        }

        [Test]
        public async Task RevokeMembership_WhenExceptionThrown_ShouldRedirectWithUnexpectedErrorMessageAndLogError()
        {
            var memberId = Guid.NewGuid();
            _membershipServiceMock
                .Setup(s => s.UpdateMembershipStatusAsync(memberId, MembershipStatus.Revoked))
                .ThrowsAsync(new Exception());

            var result = await _adminController.RevokeMembership(memberId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;

            Assert.That(redirect.ActionName, Is.EqualTo(nameof(AdminController.Members)));
            Assert.That(_adminController.TempData.ContainsKey("ErrorMessage"), Is.True);
            Assert.That(_adminController.TempData["ErrorMessage"], Is.EqualTo(UnexpectedErrorMessage));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(RevokeMembershipErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }
    }
}
