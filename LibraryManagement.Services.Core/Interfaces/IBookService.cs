namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Common;
    using LibraryManagement.Web.ViewModels.Book;
    using static LibraryManagement.GCommon.PagedResultConstants;

    /// <summary>
    /// Provides operations for managing books, including creation, retrieval, editing, and deletion.
    /// </summary>
    public interface IBookService
    {
        /// <summary>
        /// Retrieves a paginated list of books for display on the index page.
        /// </summary>
        /// <param name="searchTerm">
        /// An optional search term to filter books by title, author, or genre.  
        /// If <c>null</c>, all books will be retrieved.
        /// </param>
        /// <param name="pageNumber">The page number for pagination (default is the first page).</param>
        /// <param name="pageSize">The number of books per page.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains a <see cref="PagedResult{T}"/> of <see cref="BookIndexViewModel"/> objects.
        /// </returns>
        Task<PagedResult<BookIndexViewModel>> GetBookIndexAsync(string? searchTerm, int pageNumber = DefaultPageNumber, int pageSize = DefaultPageSize);

        /// <summary>
        /// Retrieves detailed information for a specific book, including user-specific review data.
        /// </summary>
        /// <param name="id">The unique identifier of the book.</param>
        /// <param name="userId">
        /// The unique identifier of the currently logged-in user.  
        /// Used to determine user-specific actions (e.g., whether the user has reviewed the book).
        /// </param>
        /// <param name="reviewPage">The page number for paginated reviews.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains a <see cref="BookDetailsViewModel"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<BookDetailsViewModel?> GetBookDetailsAsync(Guid id, string? userId, int reviewPage);

        /// <summary>
        /// Creates a new book in the system.
        /// </summary>
        /// <param name="userId">The unique identifier of the user creating the book.</param>
        /// <param name="inputModel">The input data required to create the book.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result is a tuple indicating whether the operation was successful  
        /// and a failure reason if unsuccessful.
        /// </returns>
        Task<(bool Success, string? FailureReason)> CreateBookAsync(string userId, BookCreateInputModel inputModel);

        /// <summary>
        /// Retrieves a book's data for editing.
        /// </summary>
        /// <param name="userId">The unique identifier of the user requesting the edit.</param>
        /// <param name="bookId">The unique identifier of the book to edit.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains a <see cref="BookEditInputModel"/> for populating the edit form.
        /// </returns>
        Task<BookEditInputModel> GetBookForEditingAsync(string userId, Guid bookId);

        /// <summary>
        /// Updates an existing book with new edited information.
        /// </summary>
        /// <param name="userId">The unique identifier of the user performing the update.</param>
        /// <param name="inputModel">The updated book data.</param>
        Task UpdateEditedBookAsync(string userId, BookEditInputModel inputModel);

        /// <summary>
        /// Marks a book as deleted (soft delete) without permanently removing it from the database.
        /// </summary>
        /// <param name="userId">The unique identifier of the user performing the delete operation.</param>
        /// <param name="inputModel">The input data for the book to delete.</param>
        Task SoftDeleteBookAsync(string userId, BookDeleteInputModel inputModel);

        /// <summary>
        /// Retrieves a book's data for deletion confirmation.
        /// </summary>
        /// <param name="userId">The unique identifier of the user requesting the delete.</param>
        /// <param name="bookId">The unique identifier of the book to delete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains a <see cref="BookDeleteInputModel"/> for the delete confirmation view.
        /// </returns>
        Task<BookDeleteInputModel> GetBookForDeletingAsync(string userId, Guid bookId);

        /// <summary>
        /// Retrieves a book by its unique identifier.
        /// </summary>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains the <see cref="Book"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<Book?> GetBookByIdAsync(Guid bookId);
    }
}
