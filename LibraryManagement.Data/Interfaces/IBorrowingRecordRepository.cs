namespace LibraryManagement.Data.Interfaces
{
    using LibraryManagement.Data.Models;
    public interface IBorrowingRecordRepository : IRepository<BorrowingRecord, Guid>
    {
        Task<IEnumerable<BorrowingRecord>> GetByMemberIdAsync(Guid memberId);

        Task<bool> HasAnyActiveBorrowAsync(Guid memberId);

        Task<bool> IsBookBorrowedAsync(Guid bookId);

        Task<BorrowingRecord?> HasActiveBorrowAsync(Guid memberId, Guid bookId);
    }
}
