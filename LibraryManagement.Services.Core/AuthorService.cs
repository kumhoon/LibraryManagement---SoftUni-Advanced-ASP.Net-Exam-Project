namespace LibraryManagement.Services.Core
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using LibraryManagement.Data.Repository;
    using LibraryManagement.Services.Common;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Author;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<PagedResult<AuthorWithBooksViewModel>> GetAuthorsWithBooksAsync(string? searchTerm, int pageNumber = 1, int pageSize = 5)
        {
            IQueryable<Author> query = _authorRepository
                .GetAllAttached()
                .Include(a => a.Books);
                
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(a => a.Name.Contains(searchTerm.Trim()));
            }

            int total = await query.CountAsync();

            var pagedAuthors = await query
                .OrderBy(a => a.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = pagedAuthors.Select(a => new AuthorWithBooksViewModel
            {
                Name = a.Name,
                Books = a.Books.Select(b => b.Title).ToList()
            });

            return new PagedResult<AuthorWithBooksViewModel>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = total
            };
            //var authors = await _authorRepository.GetAuthorsWithBooksAsync(searchTerm);

            //return authors.Select(a => new AuthorWithBooksViewModel
            //{
            //    Name = a.Name,
            //    Books = a.Books.Select(b => b.Title)
            //});
        }

        public async Task<Author> GetOrCreateAuthorAsync(string name)
        {
            var existingAuthor = await _authorRepository.GetByNameAsync(name.Trim());

            if (existingAuthor != null) 
            { 
                return existingAuthor;
            }

            var newAuthor = new Author
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
            };

            await _authorRepository.AddAsync(newAuthor);
            await _authorRepository.SaveChangesAsync();

            return newAuthor;
        }
    }
}
