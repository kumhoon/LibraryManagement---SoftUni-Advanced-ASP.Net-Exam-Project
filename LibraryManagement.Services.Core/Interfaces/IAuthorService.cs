using LibraryManagement.Data.Models;
using LibraryManagement.Web.ViewModels.Author;
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

        Task<IEnumerable<AuthorWithBooksViewModel>> GetAuthorsWithBooksAsync(string? searchTerm);
    }
}
