using LibraryManagement.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Data.Interfaces
{
    public interface IBookRepository : IRepository<Book, Guid>
    {
        Task<IEnumerable<Book>> SearchByTitleAsync(string title);
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(Guid authorId);
        Task<IEnumerable<Book>> GetBooksByGenreAsync(Guid genreId);
        Task<Book?> GetBookWithDetailsAsync(Guid id);
        Task<IEnumerable<Book>> GetPagedBooksAsync(int pageNumber, int pageSize);

        Task<IEnumerable<Book>> GetAllWithDetailsAsync();
     
    }
}
