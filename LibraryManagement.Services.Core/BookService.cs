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

        public async Task<BookDeleteInputModel?> GetBookForDeletingAsync(string userId, Guid? bookId)
        {
            BookDeleteInputModel deleteModel = null;
            if (bookId != null)
            {
                var bookDeleteModel = await _bookRepository.GetBookWithDetailsAsync(bookId.Value);
                if((bookDeleteModel != null) 
                    && (bookDeleteModel.BookCreatorId.ToLower() == userId.ToLower()))
                {
                    deleteModel = new BookDeleteInputModel
                    {
                        Title = bookDeleteModel.Title,
                        AuthorName = bookDeleteModel.Author.Name,
                    };
                }
            }
            return deleteModel;
        }

        public async Task<BookEditInputModel> GetBookForEditingAsync(string userId, Guid? bookId)
        {
            BookEditInputModel? editModel = null;
            if (bookId != null) 
            { 
                var bookEditModel = await _bookRepository.GetBookWithDetailsAsync(bookId.Value);
                if ((bookEditModel != null) && bookEditModel.BookCreatorId.ToLower() == userId.ToLower())
                {
                    editModel = new BookEditInputModel
                    {
                        Id = bookEditModel.Id,
                        Title = bookEditModel.Title,
                        Description = bookEditModel.Description,
                        ImageUrl = bookEditModel.ImageUrl,
                        GenreId = bookEditModel.GenreId,
                        PublishedDate = bookEditModel.PublishedDate,
                        Author = bookEditModel.Author.Name,
                    };
                }
            }
            return editModel;
        }

        public async Task<IEnumerable<BookIndexViewModel>> GetBookIndexAsync(string? searchTerm)
        {
            var books = await this._bookRepository.GetAllWithDetailsAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                books = books.Where(b =>
                    b.Title.ToLower().Contains(searchTerm) ||
                    b.Author.Name.ToLower().Contains(searchTerm) ||
                    b.Genre.Name.ToLower().Contains(searchTerm));
            }

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

        public async Task<bool> SoftDeleteBookAsync(string userId, BookDeleteInputModel inputModel)
        {
            bool deleteResult = false;

            IdentityUser? user = await this._userManager.FindByIdAsync(userId);

            Book? book = await this._bookRepository.GetBookWithDetailsAsync(inputModel.Id);

            if ((user != null) 
                && (book != null) 
                && (book.BookCreatorId.ToLower() == userId.ToLower())) 
            { 
                book.IsDeleted = true;
                await this._bookRepository.SaveChangesAsync();
                deleteResult = true;
            }
            return deleteResult;
        }

        public async Task<bool> UpdateEditedBookAsync(string userId, BookEditInputModel inputModel)
        {
            bool updateResult = false;

            IdentityUser? user = await this._userManager
                .FindByIdAsync(userId);

            Genre? genre = await this._genreRepository.GetByIdAsync(inputModel.GenreId);

            Book? book = await this._bookRepository.GetBookWithDetailsAsync(inputModel.Id);

            bool isPublishedOnDateValid = DateTime.TryParseExact(
                inputModel.PublishedDate.ToString(PublishedOnDateTimeFormat),
                PublishedOnDateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime PublishedOn);

            if ((user != null) 
                && (genre != null) 
                && (book != null) 
                && (book.BookCreatorId.ToLower() == userId.ToLower()))
            {
                book.Title = inputModel.Title;
                book.Author.Name = inputModel.Author;
                book.Description = inputModel.Description;
                book.ImageUrl = inputModel.ImageUrl;
                book.PublishedDate = inputModel.PublishedDate;
                book.GenreId = inputModel.GenreId;

                await this._bookRepository.UpdateAsync(book);
                updateResult = true;
            }
            return updateResult;
        }
    }
}
