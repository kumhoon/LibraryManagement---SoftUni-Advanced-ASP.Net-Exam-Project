namespace LibraryManagement.Services.Core
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Core.Interfaces;
    using System.Threading.Tasks;

    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
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
