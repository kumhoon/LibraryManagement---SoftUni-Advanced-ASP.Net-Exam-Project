using LibraryManagement.Data.Models;
using LibraryManagement.Services.Common;
using LibraryManagement.Web.ViewModels.Author;
using LibraryManagement.Web.ViewModels.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Services.Core.Interfaces
{
    public interface IAuthorService
    {
        Task<Author> GetOrCreateAuthorAsync(string name);

        //Task<IEnumerable<AuthorWithBooksViewModel>> GetAuthorsWithBooksAsync(string? searchTerm);

        Task<PagedResult<AuthorWithBooksViewModel>> GetAuthorsWithBooksAsync(string? searchTerm, int pageNumber = 1, int pageSize = 5);
    }
}
