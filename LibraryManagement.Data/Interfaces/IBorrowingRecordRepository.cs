namespace LibraryManagement.Data.Interfaces
{
    using LibraryManagement.Data.Models;

    /// <summary>
    /// Defines data access methods for borrowing records.
    /// </summary>
    public interface IBorrowingRecordRepository : IRepository<BorrowingRecord, Guid>
    {
        /// <summary>
        /// Retrieves all borrowing records associated with a specific member.
        /// </summary>
        /// <param name="memberId">The unique identifier of the library member.</param>
        /// <returns>
        /// A collection of borrowing records for the specified member.
        /// </returns>
        Task<IEnumerable<BorrowingRecord>> GetByMemberIdAsync(Guid memberId);

        /// <summary>
        /// Checks if the specified member has any currently active borrowings.
        /// </summary>
        /// <param name="memberId">The unique identifier of the library member.</param>
        /// <returns>
        /// <c>true</c> if the member has at least one active borrow; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> HasAnyActiveBorrowAsync(Guid memberId);

        /// <summary>
        /// Checks if a specific book is currently borrowed by any member.
        /// </summary>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <returns>
        /// <c>true</c> if the book is currently borrowed; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> IsBookBorrowedAsync(Guid bookId);

        /// <summary>
        /// Checks if a specific member currently has an active borrow for a given book.
        /// </summary>
        /// <param name="memberId">The unique identifier of the library member.</param>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <returns>
        /// <c>true</c> if the member has an active borrow of the book; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> HasActiveBorrowAsync(Guid memberId, Guid bookId);

        /// <summary>
        /// Retrieves the active borrowing record for a specific member and book, if any.
        /// </summary>
        /// <param name="memberId">The unique identifier of the library member.</param>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <returns>
        /// The active <see cref="BorrowingRecord"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<BorrowingRecord?> GetActiveBorrowRecordAsync(Guid memberId, Guid bookId);
    }
}
