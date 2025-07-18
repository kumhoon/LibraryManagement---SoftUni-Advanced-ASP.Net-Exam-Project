namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Common;
    using LibraryManagement.Web.ViewModels.Author;
    public interface IAuthorService
    {
        Task<Author> GetOrCreateAuthorAsync(string name);

        //Task<IEnumerable<AuthorWithBooksViewModel>> GetAuthorsWithBooksAsync(string? searchTerm);

        Task<PagedResult<AuthorWithBooksViewModel>> GetAuthorsWithBooksAsync(string? searchTerm, int pageNumber = 1, int pageSize = 5);
    }
}
