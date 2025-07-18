namespace LibraryManagement.Data.Interfaces
{
    using LibraryManagement.Data.Models;

    public interface IGenreRepository : IRepository<Genre, Guid>
    {
        Task<IEnumerable<Genre>> GetAllGenresAsync();
    }
}
