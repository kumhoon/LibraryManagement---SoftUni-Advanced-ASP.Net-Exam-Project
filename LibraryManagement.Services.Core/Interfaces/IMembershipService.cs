namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Web.ViewModels.Membership;

    /// <summary>
    /// Provides operations for handling membership applications and managing member information.
    /// </summary>
    public interface IMembershipService
    {
        /// <summary>
        /// Retrieves all pending membership applications.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains a collection of <see cref="MembershipPendingViewModel"/> objects  
        /// representing pending applications.
        /// </returns>
        Task<IEnumerable<MembershipPendingViewModel>> GetPendingApplications();

        /// <summary>
        /// Submits a membership application for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user applying for membership.</param>
        /// <param name="inputModel">The input data for the membership application.</param>
        Task ApplyForMembershipAsync(string userId, MemberApplicationInputModel inputModel);

        /// <summary>
        /// Retrieves membership details for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains the <see cref="Member"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<Member?> GetMembershipByUserIdAsync(string userId);

        /// <summary>
        /// Updates the membership status for a specific member.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member whose status is updated.</param>
        /// <param name="newStatus">The new membership status to apply.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result is <c>true</c> if the update was successful; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> UpdateMembershipStatusAsync(Guid memberId, MembershipStatus newStatus);

        /// <summary>
        /// Retrieves all approved members.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains a collection of <see cref="ApprovedMemberViewModel"/> objects  
        /// representing approved members.
        /// </returns>
        Task<IEnumerable<ApprovedMemberViewModel>> GetApprovedMembersAsync();
    }
}
