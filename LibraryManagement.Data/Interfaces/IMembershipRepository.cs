namespace LibraryManagement.Data.Interfaces
{
    using LibraryManagement.Data.Models;

    /// <summary>
    /// Defines data access methods for library membership entities.
    /// </summary>
    public interface IMembershipRepository : IRepository<Member, Guid>
    {
        /// <summary>
        /// Retrieves all members whose membership applications are pending approval.
        /// </summary>
        /// <returns>
        /// A collection of members with pending applications.
        /// </returns>
        Task<IEnumerable<Member>> GetPendingApplicationsAsync();

        /// <summary>
        /// Retrieves all members whose memberships have been approved.
        /// </summary>
        /// <returns>A collection of approved members.</returns>
        Task<IEnumerable<Member>> GetApprovedMembersAsync();

        /// <summary>
        /// Retrieves a member by their associated user ID.
        /// </summary>
        /// <param name="userId">The unique user identifier linked to the member.</param>
        /// <returns>
        /// The matching <see cref="Member"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<Member?> GetByUserIdAsync(string userId);
    }
}
