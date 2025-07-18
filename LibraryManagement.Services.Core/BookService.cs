using LibraryManagement.Data.Interfaces;
using LibraryManagement.Data.Models;
using LibraryManagement.Services.Common;
using LibraryManagement.Services.Core.Interfaces;
using LibraryManagement.Web.ViewModels.Book;
using LibraryManagement.Web.ViewModels.Review;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMembershipService _membershipService;
        private readonly IBorrowingRecordService _borrowingRecordService;
        private readonly IReviewService _reviewService;

        public BookService
            (UserManager<IdentityUser> userManager, 
            IBookRepository bookRepository, 
            IGenreService genreService, 
            IAuthorService authorService,
            IGenreRepository genreRepository,
            IMembershipService membershipService,
            IBorrowingRecordService borrowingRecordService,
            IReviewService reviewService
            )
        {
            _bookRepository = bookRepository;
            _userManager = userManager;
            _genreService = genreService;
            _authorService = authorService;
            _genreRepository = genreRepository;
            _membershipService = membershipService;
            _borrowingRecordService = borrowingRecordService;
            _reviewService = reviewService;
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

        public async Task<Book?> GetBookByIdAsync(Guid bookId)
        {
            return await _bookRepository.GetByIdAsync(bookId);
        }

        public async Task<BookDetailsViewModel?> GetBookDetailsAsync(Guid id, string? userId, int reviewPage)
        {
            var book = await this._bookRepository.GetBookWithDetailsAsync(id);

            if (book == null) { return null; }

            var viewModel = new BookDetailsViewModel
            {
                Id = book.Id,
                AuthorName = book.Author.Name,
                Description = book.Description,
                ImageUrl = book.ImageUrl,
                PublishedDate = book.PublishedDate,
                Title = book.Title,
                Genre = book.Genre.Name,
                CreatorId = book.BookCreatorId,
                IsApprovedMember = false,
                HasBorrowedThisBook = false,
                HasBorrowedAnyBook = false
            };

            int pageSize = 5;

            
            var pagedReviews = await _reviewService.GetBookReviewsAsync(id, reviewPage, pageSize);

            ReviewViewModel? memberReview = null;

            if (!string.IsNullOrEmpty(userId))
            {
                var member = await _membershipService.GetMembershipByUserIdAsync(userId);
                if (member != null && member.Status == MembershipStatus.Approved)
                {
                    viewModel.IsApprovedMember = true;

                    bool hasBorrowed = await _borrowingRecordService
                        .IsBookBorrowedByMemberAsync(member.Id, book.Id);

                    viewModel.HasBorrowedThisBook = hasBorrowed;

                    viewModel.HasBorrowedAnyBook = await _borrowingRecordService
                        .HasAnyActiveBorrowAsync(member.Id);

                    memberReview = await _reviewService.GetMemberReviewForBookAsync(member.Id, book.Id);
                }
            }

            viewModel.Reviews = pagedReviews; 

            viewModel.MemberReview = memberReview;

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

        public async Task<PagedResult<BookIndexViewModel>> GetBookIndexAsync(string? searchTerm, int pageNumber = 1, int pageSize = 5)
        {          
            IQueryable<Book> query = _bookRepository
                .GetAllAttached()              
                .Include(b => b.Author)
                .Include(b => b.Genre);

            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(b =>
                    b.Title.Contains(searchTerm.Trim()) ||
                    b.Author.Name.Contains(searchTerm) ||
                    b.Genre.Name.Contains(searchTerm));
            }

            
            int total = await query.CountAsync();

            
            var pagedBooks = await query
                .OrderBy(b => b.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            
            var items = pagedBooks.Select(b => new BookIndexViewModel
            {
                Id = b.Id,
                Title = b.Title,
                AuthorName = b.Author.Name,
                Genre = b.Genre.Name,
                PublishedDate = b.PublishedDate,
                ImageUrl = b.ImageUrl
            });

            
            return new PagedResult<BookIndexViewModel>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = total
            };
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
