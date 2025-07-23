namespace LibraryManagement.Data.Interfaces
{
    using LibraryManagement.Data.Models;
    public interface IBorrowingRecordRepository : IRepository<BorrowingRecord, Guid>
    {
        Task<IEnumerable<BorrowingRecord>> GetByMemberIdAsync(Guid memberId);

        Task<bool> HasAnyActiveBorrowAsync(Guid memberId);

        Task<bool> IsBookBorrowedAsync(Guid bookId);

        Task<bool> HasActiveBorrowAsync(Guid memberId, Guid bookId);

        Task<BorrowingRecord?> GetActiveBorrowRecordAsync(Guid memberId, Guid bookId);
    }
}
