
namespace LibraryManagement.Data.Repository
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class ReviewRepository : BaseRepository<Review, Guid>, IReviewRepository
    {
        private readonly LibraryManagementDbContext _dbContext;
        public ReviewRepository(LibraryManagementDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ApproveAsync(Guid reviewId)
        {
            var review = await _dbContext.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            review.IsApproved = true;
            _dbContext.Reviews.Update(review);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Review>> GetAllForBookAsync(Guid bookId)
        {
            return await _dbContext.Reviews
                .AsNoTracking()
                .Include(r => r.Member)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.CreatedAt)
                .ToArrayAsync();
        }

        public async Task<IEnumerable<Review>> GetApprovedByBookAsync(Guid bookId)
        {
            return await _dbContext.Reviews
                .AsNoTracking()
                .Include(r => r.Member)
                .Where(r => r.BookId == bookId && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToArrayAsync();
        }

        public async Task<IEnumerable<Review>> GetByMemberAsync(Guid memberId)
        {
            return await _dbContext.Reviews
                .AsNoTracking()
                .Include(r => r.Book)
                .Where(r => r.MemberId == memberId)
                .OrderByDescending(r => r.CreatedAt)
                .ToArrayAsync();
        }

        public async Task<IEnumerable<Review>> GetPendingAsync()
        {
            return await _dbContext.Reviews
                .AsNoTracking()
                .Include(r => r.Member)
                .Where(r => !r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToArrayAsync();
        }

        public async Task<bool> RejectAsync(Guid reviewId)
        {
            var review = await _dbContext.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            _dbContext.Reviews.Remove(review);

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
