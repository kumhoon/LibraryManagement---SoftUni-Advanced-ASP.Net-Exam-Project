using LibraryManagement.Data.Interfaces;
using LibraryManagement.Services.Core.Interfaces;
using LibraryManagement.Web.ViewModels.Book;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Services.Core
{
    
    public class BookService : IBookService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IBookRepository _bookRepository;

        public BookService(UserManager<IdentityUser> userManager, IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
            _userManager = userManager;

        }

        public Task<bool> CreateBookAsync(Guid userId, BookCreateInputModel inputModel)
        {
            throw new NotImplementedException();
        }

        public Task<BookDetailsViewModel?> GetBookDetailsAsync(Guid? id, Guid? userId)
        {
            throw new NotImplementedException();
        }

        public Task<BookDeleteInputModel> GetBookForDeletingAsync(Guid userId, Guid? bookId)
        {
            throw new NotImplementedException();
        }

        public Task<BookEditInputModel> GetBookForEditingAsync(Guid userId, Guid? bookId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BookIndexViewModel>> GetBookIndexAsync(string? userId)
        {
            var books = await this._bookRepository.GetAllWithDetailsAsync();

            return books
                .Select(book => new BookIndexViewModel
                {
                    Id = book.Id,
                    Title = book.Title,
                    AuthorName = book.Author.Name,
                    Genre = book.Genre.Name,
                    PublishedDate = book.PublishedDate,
                });
        }

        public Task<bool> SoftDeleteBookAsync(Guid userId, BookDeleteInputModel inputModel)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateEditedBookAsync(Guid userId, BookEditInputModel inputModel)
        {
            throw new NotImplementedException();
        }
    }
}
