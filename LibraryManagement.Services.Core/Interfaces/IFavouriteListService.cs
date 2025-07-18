namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Web.ViewModels.Book;

    public interface IFavouriteListService
    {
        Task<bool> AddToFavouritesAsync(Guid memberId, Guid bookId);
        Task<bool> RemoveFromFavouritesAsync(Guid memberId, Guid bookId);
        Task<bool> IsBookFavouriteAsync(Guid memberId, Guid bookId);
        Task<IEnumerable<BookIndexViewModel>> GetFavouriteBooksAsync(Guid memberId);
    }
}
