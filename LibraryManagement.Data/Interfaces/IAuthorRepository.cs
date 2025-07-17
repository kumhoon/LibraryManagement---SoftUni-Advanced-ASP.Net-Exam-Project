namespace LibraryManagement.Data.Interfaces
{
    using LibraryManagement.Data.Models;

    public interface IAuthorRepository : IRepository<Author, Guid>
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
        Task<Author?> GetByNameAsync(string name);

        Task<IEnumerable<Author>> GetAuthorsWithBooksAsync(string? searchTerm);
    }
}
