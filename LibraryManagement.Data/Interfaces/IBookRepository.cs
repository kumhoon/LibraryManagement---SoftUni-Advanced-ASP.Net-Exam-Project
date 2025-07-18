namespace LibraryManagement.Data.Interfaces
{
    using LibraryManagement.Data.Models;

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
