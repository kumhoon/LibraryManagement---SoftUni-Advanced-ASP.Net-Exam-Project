namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Web.ViewModels.BorrowingRecord;
    public interface IBorrowingRecordService
    {
        Task<BorrowResult> BorrowBookAsync(Guid memberId, Guid bookId);

        Task<bool> ReturnBookAsync(Guid memberId, Guid bookId);

        Task<IEnumerable<BorrowingRecordViewModel>> GetBorrowingHistoryAsync(Guid memberId);

        Task<bool> IsBookBorrowedByMemberAsync(Guid memberId, Guid bookId);

        Task<bool> HasAnyActiveBorrowAsync(Guid memberId);
        Task<bool> IsBookBorrowedAsync(Guid bookId);
    }

    public enum BorrowResult
    {
        Success,
        AlreadyBorrowedByMember,
        BookUnavailable,
        Failed
    }
}
