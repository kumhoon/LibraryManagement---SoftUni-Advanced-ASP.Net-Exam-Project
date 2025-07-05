namespace LibraryManagement.Data.Repository
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class AuthorRepository : BaseRepository<Author, Guid>, IAuthorRepository
    {
        private readonly LibraryManagementDbContext _context;
        public AuthorRepository(LibraryManagementDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            return await this._context.Authors
                .OrderBy (a => a.Name)
                .ToArrayAsync();
        }

        public async Task<Author?> GetByNameAsync(string name)
        {
            return await _context.Authors
                .FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());
        }
    }
}
