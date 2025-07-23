namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Common;
    using LibraryManagement.Web.ViewModels.Author;
    using static LibraryManagement.GCommon.PagedResultConstants;
    public interface IAuthorService
    {
        Task<Author> GetOrCreateAuthorAsync(string name);

        Task<PagedResult<AuthorWithBooksViewModel>> GetAuthorsWithBooksAsync(string? searchTerm, int pageNumber = DefaultPageNumber, int pageSize = DefaultPageSize);
    }
}
