namespace LibraryManagement.Data.Repository
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    public class GenreRepository : BaseRepository<Genre, Guid>, IGenreRepository
    {
        private readonly LibraryManagementDbContext _context;
        public GenreRepository(LibraryManagementDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<IEnumerable<Genre>> GetAllGenresAsync()
        {
            return await this._context.Genres
                .OrderBy(g => g.Name)
                .ToArrayAsync();
        }
    }
}
