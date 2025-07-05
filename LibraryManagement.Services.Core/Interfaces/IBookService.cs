namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Web.ViewModels.Book;

    public interface IBookService
    {
        Task<IEnumerable<BookIndexViewModel>> GetBookIndexAsync(string? userId);

        Task <BookDetailsViewModel?> GetBookDetailsAsync(Guid? id, string? userId);

        Task<bool> CreateBookAsync(Guid userId, BookCreateInputModel inputModel);

        Task<BookEditInputModel> GetBookForEditingAsync(Guid userId, Guid? bookId);

        Task<bool> UpdateEditedBookAsync(Guid userId, BookEditInputModel inputModel);

        Task<bool> SoftDeleteBookAsync(Guid userId, BookDeleteInputModel inputModel);

        Task<BookDeleteInputModel> GetBookForDeletingAsync(Guid userId, Guid? bookId);
    }
}
