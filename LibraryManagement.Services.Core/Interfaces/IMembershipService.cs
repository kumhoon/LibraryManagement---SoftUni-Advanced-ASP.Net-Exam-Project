namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Web.ViewModels.Membership;
    public interface IMembershipService
    {
        Task<IEnumerable<Member>> GetPendingMembersAsync();

        Task ApplyForMembershipAsync(string userId, MemberApplicationInputModel inputModel);
    }
}
