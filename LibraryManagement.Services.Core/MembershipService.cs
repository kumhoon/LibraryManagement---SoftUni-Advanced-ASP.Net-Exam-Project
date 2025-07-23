namespace LibraryManagement.Services.Core
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Membership;
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static LibraryManagement.GCommon.ErrorMessages;
    using static LibraryManagement.GCommon.Defaults.Text;
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
            var user = await this._userManager.FindByIdAsync(userId) ?? throw new InvalidOperationException(UserNotFoundErrorMessage);
            var existingMembership = await this._membershipRepository
                .FirstOrDefaultAsync(m => m.UserId == userId);

            if (existingMembership != null)
            {
                if (existingMembership.Status == MembershipStatus.Pending ||
                    existingMembership.Status == MembershipStatus.Approved)
                {
                    throw new InvalidOperationException(MembershipApprovedOrPendingErrorMessage);
                }

                
                existingMembership.Name = inputModel.Name;
                existingMembership.MembershipApplicationReason = inputModel.Reason;
                existingMembership.JoinDate = DateTime.UtcNow;
                existingMembership.Status = MembershipStatus.Pending;

                await _membershipRepository.UpdateAsync(existingMembership);
                return;
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

        public async Task<IEnumerable<MembershipPendingViewModel>> GetPendingApplications()
        {
            var pendingMembers = await _membershipRepository.GetPendingApplicationsAsync();

            var result = pendingMembers.Select(m => new MembershipPendingViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.User?.Email ?? UnknownEmail,
                JoinDate = m.JoinDate,
                Reason = m.MembershipApplicationReason,
                Status = m.Status
            });

            return result;
        }

        public async Task<Member?> GetMembershipByUserIdAsync(string userId) 
        {
            return await _membershipRepository
                .FirstOrDefaultAsync(m => m.UserId == userId);
        }

        public async Task<bool> UpdateMembershipStatusAsync(Guid memberId, MembershipStatus newStatus)
        {
            var member = await _membershipRepository.GetByIdAsync(memberId);

            if (member == null)
            {
                return false;
            }

            member.Status = newStatus;
            return await _membershipRepository.UpdateAsync(member);
        }

        public async Task<IEnumerable<ApprovedMemberViewModel>> GetApprovedMembersAsync()
        {
            var members = await _membershipRepository.GetApprovedMembersAsync();

            return members.Select(m => new ApprovedMemberViewModel
            {
                Id = m.Id,
                Name = m.Name,
                JoinDate = m.JoinDate,

            });
        }
    }
}
