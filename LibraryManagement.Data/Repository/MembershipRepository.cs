namespace LibraryManagement.Data.Repository
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class MembershipRepository : BaseRepository<Member, Guid>, IMembershipRepository
    {
        private readonly LibraryManagementDbContext _dbContext;
        public MembershipRepository(LibraryManagementDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Member>> GetApprovedMembersAsync()
        {
            return await _dbContext.Memberships
                    .Include(m => m.User)
                    .Where(m => m.Status == MembershipStatus.Approved)
                    .ToArrayAsync();
        }

        public async Task<IEnumerable<Member>> GetPendingApplicationsAsync()
        {
            return await _dbContext.Memberships
                    .Include(m => m.User)
                    .Where(m => m.Status == MembershipStatus.Pending)
                    .ToArrayAsync();
        }

        public async Task<Member?> GetByUserIdAsync(string userId)
        {
            return await _dbContext.Memberships.FirstOrDefaultAsync(m => m.UserId == userId);
        }
    }
}
