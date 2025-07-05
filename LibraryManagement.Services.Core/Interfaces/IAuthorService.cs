using LibraryManagement.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Services.Core.Interfaces
{
    public interface IAuthorService
    {
        Task<Author> GetOrCreateAuthorAsync(string name);
    }
}
