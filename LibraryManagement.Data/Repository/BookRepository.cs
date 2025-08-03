namespace LibraryManagement.Data.Repository
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <inheritdoc />
    public class BookRepository : BaseRepository<Book, Guid>, IBookRepository
    {
        private readonly LibraryManagementDbContext _context;
        public BookRepository(LibraryManagementDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(Guid authorId)
        {
            return await this._context.Books
                .Where(b=> b.AuthorId == authorId)
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Book>> GetBooksByGenreAsync(Guid genreId)
        {
            return await this._context.Books
                .Where(b=> b.GenreId == genreId)
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task<Book?> GetBookWithDetailsAsync(Guid id)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Book>> GetAllWithDetailsAsync()
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Book>> GetPagedBooksAsync(int pageNumber, int pageSize)
        {
            return await _context.Books
                .OrderBy(b => b.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Book>> SearchByTitleAsync(string title)
        {
            return await this._context.Books
                .Where (b=> b.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .ToArrayAsync();
        }
    }
}
