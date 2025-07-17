using LibraryManagement.Web.ViewModels.Book;

namespace LibraryManagement.Services.Core.Interfaces
{
    public interface IFavouriteListService
    {
        Task<bool> AddToFavouritesAsync(Guid memberId, Guid bookId);
        Task<bool> RemoveFromFavouritesAsync(Guid memberId, Guid bookId);
        Task<bool> IsBookFavouriteAsync(Guid memberId, Guid bookId);
        Task<IEnumerable<BookIndexViewModel>> GetFavouriteBooksAsync(Guid memberId);
    }
}
