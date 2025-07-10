namespace LibraryManagement.Data.Interfaces
{
    using LibraryManagement.Data.Models;
    public interface IBorrowingRecordRepository : IRepository<BorrowingRecord, Guid>
    {
        Task<IEnumerable<BorrowingRecord>> GetByMemberIdAsync(Guid memberId);

        Task<BorrowingRecord?> HasActiveBorrowAsync(Guid memberId, Guid bookId);

        Task<bool> IsBookBorrowedAsync(Guid memberId, Guid bookId);
    }
}
