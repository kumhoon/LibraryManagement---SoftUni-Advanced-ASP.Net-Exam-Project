namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Web.ViewModels.BorrowingRecord;

    /// <summary>
    /// Provides operations for borrowing and returning books,
    /// as well as retrieving borrowing history and status checks.
    /// </summary>
    public interface IBorrowingRecordService
    {
        /// <summary>
        /// Borrows a book for a specific member if available.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member borrowing the book.</param>
        /// <param name="bookId">The unique identifier of the book to borrow.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result is a <see cref="BorrowResult"/> indicating the outcome of the operation.
        /// </returns>
        Task<BorrowResult> BorrowBookAsync(Guid memberId, Guid bookId);

        /// <summary>
        /// Returns a previously borrowed book for a specific member.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member returning the book.</param>
        /// <param name="bookId">The unique identifier of the book to return.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result is <c>true</c> if the return was successful; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ReturnBookAsync(Guid memberId, Guid bookId);

        /// <summary>
        /// Retrieves the borrowing history for a specific member.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member whose history is retrieved.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains a collection of <see cref="BorrowingRecordViewModel"/> objects.
        /// </returns>
        Task<IEnumerable<BorrowingRecordViewModel>> GetBorrowingHistoryAsync(Guid memberId);

        /// <summary>
        /// Checks whether a specific member has currently borrowed a specific book.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member.</param>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result is <c>true</c> if the book is currently borrowed by the member; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> IsBookBorrowedByMemberAsync(Guid memberId, Guid bookId);

        /// <summary>
        /// Checks whether a member has any active (not returned) borrowings.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result is <c>true</c> if the member has at least one active borrowing; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> HasAnyActiveBorrowAsync(Guid memberId);

        /// <summary>
        /// Checks whether a specific book is currently borrowed by any member.
        /// </summary>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result is <c>true</c> if the book is currently borrowed; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> IsBookBorrowedAsync(Guid bookId);

    }

    /// <summary>
    /// Represents the possible results of a borrow book operation.
    /// </summary>
    public enum BorrowResult
    {
        Success,
        AlreadyBorrowedByMember,
        BookUnavailable,
        Failed
    }
}
