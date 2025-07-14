namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Web.ViewModels.Membership;
    public interface IMembershipService
    {
        Task<IEnumerable<MembershipPendingViewModel>> GetPendingApplications();

        Task ApplyForMembershipAsync(string userId, MemberApplicationInputModel inputModel);

        Task<Member?> GetMembershipByUserIdAsync(string userId);

        Task<bool> UpdateMembershipStatusAsync(Guid memberId, MembershipStatus newStatus);

        Task<IEnumerable<ApprovedMemberViewModel>> GetApprovedMembersAsync();
    }
}
