using LibraryManagement.Data.Interfaces;
using LibraryManagement.Data.Models;
using LibraryManagement.Services.Core.Interfaces;
using LibraryManagement.Web.ViewModels.Book;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using static LibraryManagement.GCommon.ViewModelValidationConstants.BookConstants;

namespace LibraryManagement.Services.Core
{
    
    public class BookService : IBookService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IBookRepository _bookRepository;
        private readonly IGenreService _genreService;
        private readonly IAuthorService _authorService;
        private readonly IGenreRepository _genreRepository;

        public BookService
            (UserManager<IdentityUser> userManager, 
            IBookRepository bookRepository, 
            IGenreService genreService, 
            IAuthorService authorService,
            IGenreRepository genreRepository
            )
        {
            _bookRepository = bookRepository;
            _userManager = userManager;
            _genreService = genreService;
            _authorService = authorService;
            _genreRepository = genreRepository;
        }

        public async Task<bool> CreateBookAsync(string userId, BookCreateInputModel inputModel)
        {
            bool createResult = false;

            IdentityUser? user = await this._userManager.FindByIdAsync(userId);

            var genre = await this._genreRepository.GetByIdAsync(inputModel.GenreId);

            var author = await this._authorService.GetOrCreateAuthorAsync(inputModel.Author);

            bool isPublishedOnDateValid = DateTime.TryParseExact(
                inputModel.PublishedDate.ToString(PublishedOnDateTimeFormat),
                PublishedOnDateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime PublishedOn);

            if ((user != null) && (genre != null) && (isPublishedOnDateValid))
            {
                Book book = new Book
                {
                    Title = inputModel.Title,
                    Description = inputModel.Description,
                    ImageUrl = inputModel.ImageUrl,
                    PublishedDate = PublishedOn,
                    BookCreatorId = userId,
                    Author = author,
                    Genre = genre
                };

                await this._bookRepository.AddAsync(book);
                await this._bookRepository.SaveChangesAsync();
                createResult = true;
            }
            return createResult;
        }

        public async Task<BookDetailsViewModel?> GetBookDetailsAsync(Guid id, string? userId)
        {
            var book = await this._bookRepository.GetBookWithDetailsAsync(id);

            if (book == null) { return null;}

            var viewModel = new BookDetailsViewModel
            {
                Id = book.Id,
                AuthorName = book.Author.Name,
                Description = book.Description,
                ImageUrl = book.ImageUrl,
                PublishedDate = book.PublishedDate,
                Title = book.Title,
                Genre = book.Genre.Name,
                CreatorId = book.BookCreatorId
            };
                
            return viewModel;
        }

        public Task<BookDeleteInputModel> GetBookForDeletingAsync(Guid userId, Guid? bookId)
        {
            throw new NotImplementedException();
        }

        public Task<BookEditInputModel> GetBookForEditingAsync(Guid userId, Guid? bookId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BookIndexViewModel>> GetBookIndexAsync(string? userId)
        {
            var books = await this._bookRepository.GetAllWithDetailsAsync();

            return books
                .Select(book => new BookIndexViewModel
                {
                    Id = book.Id,
                    Title = book.Title,
                    AuthorName = book.Author.Name,
                    Genre = book.Genre.Name,
                    PublishedDate = book.PublishedDate,
                });
        }

        public Task<bool> SoftDeleteBookAsync(Guid userId, BookDeleteInputModel inputModel)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateEditedBookAsync(Guid userId, BookEditInputModel inputModel)
        {
            throw new NotImplementedException();
        }
    }
}
