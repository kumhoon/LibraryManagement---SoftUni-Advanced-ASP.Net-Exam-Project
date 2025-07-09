namespace LibraryManagement.Data.Repository
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class MembershipRepository : BaseRepository<Member, Guid>, IMembershipRepository
    {
        private readonly LibraryManagementDbContext _context;
        public MembershipRepository(LibraryManagementDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<IEnumerable<Member>> GetPendingApplicationsAsync()
        {
            return await _context.Memberships
                    .Include(m => m.User)
                    .Where(m => m.Status == MembershipStatus.Pending)
                    .ToListAsync();
        }
    }
}
