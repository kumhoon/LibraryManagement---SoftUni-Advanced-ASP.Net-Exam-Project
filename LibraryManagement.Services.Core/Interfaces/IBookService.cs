namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Web.ViewModels.Book;

    public interface IBookService
    {
        Task<IEnumerable<BookIndexViewModel>> GetBookIndexAsync(string? userId);

        Task <BookDetailsViewModel?> GetBookDetailsAsync(Guid id, string? userId);

        Task<bool> CreateBookAsync(string userId, BookCreateInputModel inputModel);

        Task<BookEditInputModel> GetBookForEditingAsync(string userId, Guid? bookId);

        Task<bool> UpdateEditedBookAsync(string userId, BookEditInputModel inputModel);

        Task<bool> SoftDeleteBookAsync(string userId, BookDeleteInputModel inputModel);

        Task<BookDeleteInputModel?> GetBookForDeletingAsync(string userId, Guid? bookId);

        Task<Book?> GetBookByIdAsync(Guid bookId);
    }
}
