namespace LibraryManagement.Services.Core
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Membership;
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public MembershipService(IMembershipRepository membershipRepository, UserManager<IdentityUser> userManager)
        {
            _membershipRepository = membershipRepository;
            _userManager = userManager;
        }

        public async Task ApplyForMembershipAsync(string userId, MemberApplicationInputModel inputModel)
        {
            var user = await this._userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            var existingMembership = await this._membershipRepository
                .FirstOrDefaultAsync(m => m.UserId == userId);

            if (existingMembership != null)
            {
                throw new InvalidOperationException("You have already applied for or have been granted membership");
            }

            var member = new Member
            {
                Id = Guid.NewGuid(),
                Name = inputModel.Name,
                MembershipApplicationReason = inputModel.Reason,
                JoinDate = DateTime.UtcNow,
                UserId = userId,
                Status = MembershipStatus.Pending,
            };

            await _membershipRepository.AddAsync(member);
            await _membershipRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<Member>> GetPendingMembersAsync()
        {
            return await _membershipRepository.GetPendingApplicationsAsync();
        }
    }
}
