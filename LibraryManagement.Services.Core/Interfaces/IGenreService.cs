namespace LibraryManagement.Services.Core.Interfaces
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// Provides operations for retrieving and managing book genres.
    /// </summary>
    public interface IGenreService
    {
        /// <summary>
        /// Retrieves all genres as a collection of select list items for use in dropdown menus.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains a collection of <see cref="SelectListItem"/> objects  
        /// representing all available genres.
        /// </returns>
        Task<IEnumerable<SelectListItem>> GetAllAsSelectListAsync();
    }
}
