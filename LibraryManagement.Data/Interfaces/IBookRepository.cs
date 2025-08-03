namespace LibraryManagement.Data.Interfaces
{
    using LibraryManagement.Data.Models;

    /// <summary>
    /// Defines data access methods for book entities.
    /// </summary>
    public interface IBookRepository : IRepository<Book, Guid>
    {
        /// <summary>
        /// Searches for books whose titles contain the specified keyword.
        /// </summary>
        /// <param name="title">The title keyword to search for.</param>
        /// <returns>
        /// A collection of books matching the search criteria.
        /// </returns>
        Task<IEnumerable<Book>> SearchByTitleAsync(string title);

        /// <summary>
        /// Retrieves all books written by the specified author.
        /// </summary>
        /// <param name="authorId"> The unique identifier of the author.</param>
        /// <returns>
        /// A collection of books by the given author.
        /// </returns>
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(Guid authorId);

        /// <summary>
        /// Retrieves all books belonging to the specified genre.
        /// </summary>
        /// <param name="genreId">The unique identifier of the genre.</param>
        /// <returns>
        /// A collection of books within the specified genre.
        /// </returns>
        Task<IEnumerable<Book>> GetBooksByGenreAsync(Guid genreId);

        /// <summary>
        /// Retrieves a book by its ID, including related details such as author and genre.
        /// </summary>
        /// <param name="id">The unique identifier of the book.</param>
        /// <returns>
        /// The book with related details if found; otherwise, <c>null</c>.
        /// </returns>
        Task<Book?> GetBookWithDetailsAsync(Guid id);

        /// <summary>
        /// Retrieves a paginated list of books.
        /// </summary>
        /// <param name="pageNumber">The page number (starting at 1).</param>
        /// <param name="pageSize">The number of books per page.</param>
        /// <returns>
        /// A collection of books for the specified page.
        /// </returns>

        Task<IEnumerable<Book>> GetPagedBooksAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Retrieves all books including related details such as authors and genres.
        /// </summary>
        /// <returns>
        /// A collection of all books with their related details.
        /// </returns>
        Task<IEnumerable<Book>> GetAllWithDetailsAsync();
     
    }
}
