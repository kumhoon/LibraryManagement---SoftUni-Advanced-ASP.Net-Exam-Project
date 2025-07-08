namespace LibraryManagement.Data.Interfaces
{
    using LibraryManagement.Data.Models;
    public interface IMembershipRepository : IRepository<Member, Guid>
    {
        Task<IEnumerable<Member>> GetPendingApplicationsAsync();
    }
}
