namespace LibraryManagement.Data.Interfaces
{
    using LibraryManagement.Data.Models;

    /// <summary>
    /// Defines data access methods for author entities.
    /// </summary>
    public interface IAuthorRepository : IRepository<Author, Guid>
    {
        /// <summary>
        /// Retrieves all authors from the database.
        /// </summary>
        /// <returns>
        /// A collection of all authors.
        /// </returns>
        Task<IEnumerable<Author>> GetAllAuthorsAsync();

        /// <summary>
        /// Finds an author by their exact name.
        /// </summary>
        /// <param name="name">The full name of the author to search for.</param>
        /// <returns>
        /// The matching <see cref="Author"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<Author?> GetByNameAsync(string name);


        /// <summary>
        /// Retrieves authors along with their associated books.
        /// </summary>
        /// <param name="searchTerm">
        /// Optional search keyword to filter authors by name. 
        /// If <c>null</c>, all authors with their books are returned.
        /// </param>
        /// <returns>
        /// A collection of authors with their related books loaded.
        /// </returns>
        Task<IEnumerable<Author>> GetAuthorsWithBooksAsync(string? searchTerm);
    }
}
