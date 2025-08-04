namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Web.ViewModels.Book;

    /// <summary>
    /// Provides operations for managing a member's list of favourite books.
    /// </summary>
    public interface IFavouriteListService
    {
        /// <summary>
        /// Adds a book to a member's list of favourites.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member.</param>
        /// <param name="bookId">The unique identifier of the book to add.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result is <c>true</c> if the book was successfully added; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> AddToFavouritesAsync(Guid memberId, Guid bookId);

        /// <summary>
        /// Removes a book from a member's list of favourites.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member.</param>
        /// <param name="bookId">The unique identifier of the book to remove.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result is <c>true</c> if the book was successfully removed; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> RemoveFromFavouritesAsync(Guid memberId, Guid bookId);

        /// <summary>
        /// Checks whether a specific book is in a member's list of favourites.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member.</param>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result is <c>true</c> if the book is in the favourites list; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> IsBookFavouriteAsync(Guid memberId, Guid bookId);

        /// <summary>
        /// Retrieves all books in a member's list of favourites.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains a collection of <see cref="BookIndexViewModel"/> objects  
        /// representing the member's favourite books.
        /// </returns>
        Task<IEnumerable<BookIndexViewModel>> GetFavouriteBooksAsync(Guid memberId);
    }
}
