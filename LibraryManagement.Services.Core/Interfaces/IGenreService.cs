namespace LibraryManagement.Services.Core.Interfaces
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    public interface IGenreService
    {
        Task<IEnumerable<SelectListItem>> GetAllAsSelectListAsync();
    }
}
