namespace LibraryManagement.Services.Core
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Common;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Book;
    using LibraryManagement.Web.ViewModels.Review;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;
    using static LibraryManagement.Web.ViewModels.ViewModelValidationConstants.BookConstants;
    using static LibraryManagement.GCommon.PagedResultConstants;
    using static LibraryManagement.GCommon.ErrorMessages;
    using LibraryManagement.GCommon;

    /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<(bool Success, string? FailureReason)> CreateBookAsync(string userId, BookCreateInputModel inputModel)
        {

            IdentityUser? user = await this._userManager.FindByIdAsync(userId);
            if (user == null) return (false, UserNotFoundErrorMessage);

            var genre = await this._genreRepository.GetByIdAsync(inputModel.GenreId);
            if (genre == null) return (false, InvalidGenreErrorMessage);

            var author = await this._authorService.GetOrCreateAuthorAsync(inputModel.Author);

            if (!DateTime.TryParseExact(
                inputModel.PublishedDate,
                PublishedOnDateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime PublishedOn))
            { 
                return (false, InvalidBookErrorMessage);
            }
            
            Book book = new Book
            {
                Title = inputModel.Title.Trim(),
                Description = inputModel.Description.Trim(),
                ImageUrl = inputModel.ImageUrl?.Trim(),
                PublishedDate = PublishedOn,
                BookCreatorId = userId,
                Author = author,
                Genre = genre
            };

            await this._bookRepository.AddAsync(book);
            await this._bookRepository.SaveChangesAsync();
            return (true, null);
        }

        /// <inheritdoc />
        public async Task<Book?> GetBookByIdAsync(Guid bookId)
        {
            return await _bookRepository.GetByIdAsync(bookId);
        }

        /// <inheritdoc />
        public async Task<BookDetailsViewModel?> GetBookDetailsAsync(Guid id, string? userId, int reviewPage)
        {
            var book = await this._bookRepository.GetBookWithDetailsAsync(id);

            if (book == null) { throw new KeyNotFoundException(BookNotFoundErrorMessage); }

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

            int pageSize = DefaultPageSize;
            
            var pagedReviews = await _reviewService.GetBookReviewsAsync(id, reviewPage, pageSize);

            ReviewInputModel? memberReview = null;

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

        /// <inheritdoc />
        public async Task<BookDeleteInputModel> GetBookForDeletingAsync(string userId, Guid bookId)
        {
            var book = await _bookRepository.GetBookWithDetailsAsync(bookId)
                ?? throw new KeyNotFoundException(BookNotFoundErrorMessage);

            if (!book.BookCreatorId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException(NotAuthorizedErrorMessage);

            return new BookDeleteInputModel
            {
                Id = book.Id,
                Title = book.Title,
                AuthorName = book.Author.Name 
            };
        }

        /// <inheritdoc />
        public async Task<BookEditInputModel> GetBookForEditingAsync(string userId, Guid bookId)
        {
            var book = await _bookRepository.GetBookWithDetailsAsync(bookId);

            if (book == null)
                throw new KeyNotFoundException(BookNotFoundErrorMessage);

            if (!book.BookCreatorId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException(NotAuthorizedErrorMessage);

            return new BookEditInputModel
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                ImageUrl = book.ImageUrl,
                GenreId = book.GenreId,
                PublishedDate = book.PublishedDate.ToString(PublishedOnDateTimeFormat),
                Author = book.Author.Name
            };
        }

        /// <inheritdoc />
        public async Task<PagedResult<BookIndexViewModel>> GetBookIndexAsync(string? searchTerm, int pageNumber = DefaultPageNumber, int pageSize = DefaultPageSize)
        {          
            PaginationValidator.Validate(pageNumber, pageSize);

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
                .ToArrayAsync();

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

        /// <inheritdoc />
        public async Task SoftDeleteBookAsync(string userId, BookDeleteInputModel inputModel)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException(UserNotFoundErrorMessage);

            var book = await _bookRepository.GetBookWithDetailsAsync(inputModel.Id)
                ?? throw new KeyNotFoundException(BookNotFoundErrorMessage);

            if (!book.BookCreatorId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException(NotAuthorizedErrorMessage);

            book.IsDeleted = true;
            await _bookRepository.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpdateEditedBookAsync(string userId, BookEditInputModel inputModel)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException(UserNotFoundErrorMessage);

            var genre = await _genreRepository.GetByIdAsync(inputModel.GenreId)
                ?? throw new InvalidOperationException(GenreNotFoundErrorMessage);

            var book = await _bookRepository.GetBookWithDetailsAsync(inputModel.Id)
                ?? throw new KeyNotFoundException(BookNotFoundErrorMessage);

            if (!book.BookCreatorId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException(NotAuthorizedErrorMessage);

            if (!DateTime.TryParseExact(
                    inputModel.PublishedDate,
                    PublishedOnDateTimeFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime publishedOn))
            {
                throw new FormatException(InvalidPublishedDateFormatErrorMessage);
            }

            book.Title = inputModel.Title;
            book.Author.Name = inputModel.Author;
            book.Description = inputModel.Description;
            book.ImageUrl = inputModel.ImageUrl;
            book.PublishedDate = publishedOn;
            book.GenreId = inputModel.GenreId;

            await _bookRepository.UpdateAsync(book);
        }
    }
}
