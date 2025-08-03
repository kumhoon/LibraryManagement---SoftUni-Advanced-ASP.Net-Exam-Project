namespace LibraryManagement.Data.Interfaces
{
    using LibraryManagement.Data.Models;

    /// <summary>
    /// Defines data access methods for genre entities.
    /// </summary>
    public interface IGenreRepository : IRepository<Genre, Guid>
    {
        /// <summary>
        /// Retrieves all genres available in the system.
        /// </summary>
        /// <returns>
        /// A collection of all genres.
        /// </returns>
        Task<IEnumerable<Genre>> GetAllGenresAsync();
    }
}
