namespace LibraryManagement.Services.Core
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Author;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<IEnumerable<AuthorWithBooksViewModel>> GetAuthorsWithBooksAsync(string? searchTerm)
        {
            var authors = await _authorRepository.GetAuthorsWithBooksAsync(searchTerm);

            return authors.Select(a => new AuthorWithBooksViewModel
            {
                Name = a.Name,
                Books = a.Books.Select(b => b.Title)
            });
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
