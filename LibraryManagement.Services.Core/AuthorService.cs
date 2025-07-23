namespace LibraryManagement.Services.Core
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using LibraryManagement.GCommon;
    using LibraryManagement.Services.Common;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Author;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using static LibraryManagement.GCommon.ErrorMessages;
    using static LibraryManagement.GCommon.PagedResultConstants;
    using static LibraryManagement.GCommon.PaginationValidator;

    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<PagedResult<AuthorWithBooksViewModel>> GetAuthorsWithBooksAsync(string? searchTerm, int pageNumber = DefaultPageNumber, int pageSize = DefaultPageSize)
        {
            PaginationValidator.Validate(pageNumber, pageSize);

            IQueryable<Author> query = _authorRepository
                .GetAllAttached()
                .Where(a => a.Books.Any(b => !b.IsDeleted)) 
                .Include(a => a.Books.Where(b => !b.IsDeleted));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(a => a.Name.Contains(searchTerm.Trim()));
            }

            int total = await query.CountAsync();

            var pagedAuthors = await query
                .OrderBy(a => a.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();

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
        }

        public async Task<Author> GetOrCreateAuthorAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(BookAuthorNameErrorMessage, nameof(name));
            }

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
