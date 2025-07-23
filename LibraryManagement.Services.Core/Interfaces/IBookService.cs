namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Common;
    using LibraryManagement.Web.ViewModels.Book;
    using static LibraryManagement.GCommon.PagedResultConstants;
    public interface IBookService
    {
        
        Task<PagedResult<BookIndexViewModel>> GetBookIndexAsync(string? searchTerm, int pageNumber = DefaultPageNumber, int pageSize = DefaultPageSize);

        Task<BookDetailsViewModel?> GetBookDetailsAsync(Guid id, string? userId, int reviewPage);

        Task<(bool Success, string? FailureReason)> CreateBookAsync(string userId, BookCreateInputModel inputModel);

        Task<BookEditInputModel> GetBookForEditingAsync(string userId, Guid bookId);

        Task UpdateEditedBookAsync(string userId, BookEditInputModel inputModel);

        Task SoftDeleteBookAsync(string userId, BookDeleteInputModel inputModel);

        Task<BookDeleteInputModel> GetBookForDeletingAsync(string userId, Guid bookId);

        Task<Book?> GetBookByIdAsync(Guid bookId);
    }
}
