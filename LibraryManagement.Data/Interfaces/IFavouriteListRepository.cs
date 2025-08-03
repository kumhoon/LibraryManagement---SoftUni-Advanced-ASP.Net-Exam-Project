namespace LibraryManagement.Data.Interfaces
{
    using LibraryManagement.Data.Models;

    /// <summary>
    /// Defines data access methods for managing members' favourite book lists.
    /// </summary>
    public interface IFavouriteListRepository : IRepository<FavouriteList, Guid>
    {
        /// <summary>
        /// Adds a book to the specified member's favourite list.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member.</param>
        /// <param name="bookId">The unique identifier of the book to add.</param>
        /// <returns>
        /// <c>true</c> if the book was successfully added; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> AddAsync(Guid memberId, Guid bookId);

        /// <summary>
        /// Removes a book from the specified member's favourite list.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member.</param>
        /// <param name="bookId">The unique identifier of the book to remove.</param>
        /// <returns>
        /// <c>true</c> if the book was successfully removed; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> RemoveAsync(Guid memberId, Guid bookId);

        /// <summary>
        /// Checks if a book exists in the specified member's favourite list.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member.</param>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <returns>
        /// <c>true</c> if the book exists in the favourite list; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ExistsAsync(Guid memberId, Guid bookId);

        /// <summary>
        /// Retrieves all favourite books for the specified member.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member.</param>
        /// <returns>
        /// A collection of the member's favourite books.
        /// </returns>
        Task<IEnumerable<Book>> GetFavouriteBooksAsync(Guid memberId);   

    }
}
