using LibraryManagement.Data.Interfaces;
using LibraryManagement.Services.Core.Interfaces;
using System;
namespace LibraryManagement.Services.Core
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }
        public async Task<IEnumerable<SelectListItem>> GetAllAsSelectListAsync()
        {
            var genres = await this._genreRepository.GetAllAsync();

            return genres
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Name,
                });
        }
    }
}
