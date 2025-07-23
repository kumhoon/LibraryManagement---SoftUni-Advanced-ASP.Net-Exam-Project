namespace LibraryManagement.Data.Repository
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class BorrowingRecordRepository : BaseRepository<BorrowingRecord, Guid>, IBorrowingRecordRepository
    {
        private readonly LibraryManagementDbContext _context;
        public BorrowingRecordRepository(LibraryManagementDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<IEnumerable<BorrowingRecord>> GetByMemberIdAsync(Guid memberId)
        {
            return await this._context.BorrowingRecords
                .Include(b => b.Book)
                .Where(br  => br.MemberId == memberId)
                .ToArrayAsync();       
                
        }

        public async Task<bool> HasAnyActiveBorrowAsync(Guid memberId)
        {
            return await _context.BorrowingRecords
                .AnyAsync(br => br.MemberId == memberId &&
                                br.ReturnDate == null);
        }

        public async Task<bool> IsBookBorrowedAsync(Guid bookId)
        {
            return await _context.BorrowingRecords
                .AnyAsync(br => br.BookId == bookId &&
                                br.ReturnDate == null);

        }

        public async Task<bool> HasActiveBorrowAsync(Guid memberId, Guid bookId)
        {
            return await _context.BorrowingRecords
                .AnyAsync(br => br.MemberId == memberId &&
                                br.BookId == bookId &&
                                br.ReturnDate == null);
        }

        public async Task<BorrowingRecord?> GetActiveBorrowRecordAsync(Guid memberId, Guid bookId)
        {
            return await _context.BorrowingRecords
                .FirstOrDefaultAsync(br => br.MemberId == memberId &&
                                           br.BookId == bookId &&
                                           br.ReturnDate == null);
        }
    }

}
