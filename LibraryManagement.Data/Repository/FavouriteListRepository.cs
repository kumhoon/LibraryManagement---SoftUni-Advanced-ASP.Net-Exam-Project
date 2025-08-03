namespace LibraryManagement.Data.Repository
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using Microsoft.EntityFrameworkCore;

    /// <inheritdoc />
    public class FavouriteListRepository : BaseRepository<FavouriteList, Guid>, IFavouriteListRepository
    {
        private readonly LibraryManagementDbContext _dbContext;

        public FavouriteListRepository(LibraryManagementDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<bool> AddAsync(Guid memberId, Guid bookId)
        {
            var exists = await ExistsAsync(memberId, bookId);
            if (exists) return false;

            var fav = new FavouriteList
            {
                MemberId = memberId,
                BookId = bookId,
                AddedAt = DateTime.UtcNow
            };

            await base.AddAsync(fav); 
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> RemoveAsync(Guid memberId, Guid bookId)
        {
            var fav = await _dbContext.FavouriteLists
                    .FirstOrDefaultAsync(f => f.MemberId == memberId && f.BookId == bookId);

            if (fav == null) return false;

            _dbContext.FavouriteLists.Remove(fav); 
            await _dbContext.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(Guid memberId, Guid bookId)
        {
            return await _dbContext.FavouriteLists
                .AnyAsync(f => f.MemberId == memberId && f.BookId == bookId);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Book>> GetFavouriteBooksAsync(Guid memberId)
        {
            return await _dbContext.FavouriteLists
                        .Where(f => f.MemberId == memberId)
                        .Include(f => f.Book)                 
                          .ThenInclude(b => b.Author)      
                        .Include(f => f.Book)
                          .ThenInclude(b => b.Genre)       
                        .Select(f => f.Book)
                        .ToArrayAsync();
        }

    }
}

