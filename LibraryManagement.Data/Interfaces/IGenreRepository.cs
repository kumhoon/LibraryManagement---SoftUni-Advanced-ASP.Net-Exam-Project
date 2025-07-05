using LibraryManagement.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Data.Interfaces
{
    public interface IGenreRepository : IRepository<Genre, Guid>
    {
        Task<IEnumerable<Genre>> GetAllGenresAsync();
    }
}
