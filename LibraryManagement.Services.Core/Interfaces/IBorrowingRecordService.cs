namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Web.ViewModels.BorrowingRecord;

    public interface IBorrowingRecordService
    {
        Task<bool> BorrowBookAsync(Guid memberId, Guid bookId);

        Task<bool> ReturnBookAsync(Guid memberId, Guid bookId);

        Task<IEnumerable<BorrowingRecordViewModel>> GetBorrowingHistoryAsync(Guid memberId);

        Task<bool> IsBookBorrowedAsync(Guid memberId, Guid bookId);
    }
}
