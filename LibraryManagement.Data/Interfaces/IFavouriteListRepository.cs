namespace LibraryManagement.Data.Interfaces
{
    using LibraryManagement.Data.Models;

    public interface IFavouriteListRepository : IRepository<FavouriteList, Guid>
    {
        Task<bool> AddAsync(Guid memberId, Guid bookId);

        Task<bool> RemoveAsync(Guid memberId, Guid bookId);

        Task<bool> ExistsAsync(Guid memberId, Guid bookId);

        Task<IEnumerable<Book>> GetFavouriteBooksAsync(Guid memberId);   

    }
}
