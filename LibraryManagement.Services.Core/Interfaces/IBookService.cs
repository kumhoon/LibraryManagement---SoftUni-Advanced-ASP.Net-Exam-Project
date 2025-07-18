namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Common;
    using LibraryManagement.Web.ViewModels.Book;
    public interface IBookService
    {
        
        Task<PagedResult<BookIndexViewModel>> GetBookIndexAsync(string? searchTerm, int pageNumber = 1, int pageSize = 5);

        Task<BookDetailsViewModel?> GetBookDetailsAsync(Guid id, string? userId, int reviewPage);

        Task<bool> CreateBookAsync(string userId, BookCreateInputModel inputModel);

        Task<BookEditInputModel?> GetBookForEditingAsync(string userId, Guid? bookId);

        Task<bool> UpdateEditedBookAsync(string userId, BookEditInputModel inputModel);

        Task<bool> SoftDeleteBookAsync(string userId, BookDeleteInputModel inputModel);

        Task<BookDeleteInputModel?> GetBookForDeletingAsync(string userId, Guid? bookId);

        Task<Book?> GetBookByIdAsync(Guid bookId);
    }
}
