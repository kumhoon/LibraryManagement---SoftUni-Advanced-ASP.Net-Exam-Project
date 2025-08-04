namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Common;
    using LibraryManagement.Web.ViewModels.Author;
    using static LibraryManagement.GCommon.PagedResultConstants;

    /// <summary>
    /// Provides operations for managing authors and retrieving related data.
    /// </summary>
    public interface IAuthorService
    {
        /// <summary>
        /// Retrieves an existing author by name or creates a new author if none is found.
        /// </summary>
        /// <param name="name">The name of the author to retrieve or create.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains the retrieved or newly created <see cref="Author"/>.
        /// </returns>
        Task<Author> GetOrCreateAuthorAsync(string name);

        /// <summary>
        /// Retrieves a paginated list of authors along with their associated books.
        /// </summary>
        /// <param name="searchTerm">
        /// An optional search term to filter authors by name.  
        /// If <c>null</c>, all authors will be retrieved.
        /// </param>
        /// <param name="pageNumber">The page number for pagination (default is the first page).</param>
        /// <param name="pageSize">The number of authors per page.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains a <see cref="PagedResult{T}"/> of <see cref="AuthorWithBooksViewModel"/> objects.
        /// </returns>
        Task<PagedResult<AuthorWithBooksViewModel>> GetAuthorsWithBooksAsync(string? searchTerm, int pageNumber = DefaultPageNumber, int pageSize = DefaultPageSize);
    }
}
