namespace LibraryManagement.Services.Core.Tests.Services.BookServiceTests
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Common;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Book;
    using LibraryManagement.Web.ViewModels.Review;
    using Microsoft.AspNetCore.Identity;
    using Moq;
    using static LibraryManagement.GCommon.ErrorMessages;
    [TestFixture]
    public class BookServiceTests
    {
        private Mock<UserManager<IdentityUser>> _userManagerMock;
        private Mock<IBookRepository> _bookRepositoryMock;
        private Mock<IGenreService> _genreServiceMock;
        private Mock<IAuthorService> _authorServiceMock;
        private Mock<IGenreRepository> _genreRepositoryMock;
        private Mock<IMembershipService> _membershipServiceMock;
        private Mock<IBorrowingRecordService> _borrowingRecordServiceMock;
        private Mock<IReviewService> _reviewServiceMock;

        private BookService _bookService;

        [SetUp]
        public void Setup()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            _bookRepositoryMock = new Mock<IBookRepository>();
            _genreServiceMock = new Mock<IGenreService>();
            _authorServiceMock = new Mock<IAuthorService>();
            _genreRepositoryMock = new Mock<IGenreRepository>();
            _membershipServiceMock = new Mock<IMembershipService>();
            _borrowingRecordServiceMock = new Mock<IBorrowingRecordService>();
            _reviewServiceMock = new Mock<IReviewService>();

            _bookService = new BookService(
                _userManagerMock.Object,
                _bookRepositoryMock.Object,
                _genreServiceMock.Object,
                _authorServiceMock.Object,
                _genreRepositoryMock.Object,
                _membershipServiceMock.Object,
                _borrowingRecordServiceMock.Object,
                _reviewServiceMock.Object
            );
        }

        [Test]
        public async Task CreateBookAsync_ShouldSucceed_WhenValidInput()
        {

            string userId = "test-user-id";
            Guid genreId = Guid.NewGuid();
            Guid authorId = Guid.NewGuid();
            var input = new BookCreateInputModel
            {
                Title = "Test Title",
                Description = "Test Description",
                ImageUrl = "http://example.com/image.jpg",
                PublishedDate = "01-01-2004",
                GenreId = genreId,
                Author = "Test Author"
            };

            _userManagerMock
               .Setup(x => x.FindByIdAsync(userId))
               .ReturnsAsync(new IdentityUser());

            _genreRepositoryMock
               .Setup(x => x.GetByIdAsync(input.GenreId))
               .ReturnsAsync(new Genre { Id = genreId, Name = "Fiction" });

            _authorServiceMock
                .Setup(x => x.GetOrCreateAuthorAsync(input.Author))
                .ReturnsAsync(new Author { Id = authorId, Name = input.Author });

            _bookRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Book>()))
                .Returns(Task.CompletedTask);

            _bookRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var (success, failureReason) = await _bookService.CreateBookAsync(userId, input);

            Assert.IsTrue(success);
            Assert.IsNull(failureReason);

            _bookRepositoryMock.Verify(x => x.AddAsync(It.Is<Book>(b =>
                b.Title == input.Title &&
                b.Description == input.Description &&
                b.ImageUrl == input.ImageUrl &&
                b.Author.Name == input.Author &&
                b.Genre.Id == input.GenreId
            )), Times.Once);

            _bookRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task CreateBookAsync_ShouldFail_WhenUserNotFound()
        {
            Guid genreId = Guid.NewGuid();

            _userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityUser)null);

            var input = new BookCreateInputModel { GenreId = genreId, PublishedDate = "2024-01-01" };

            var (success, reason) = await _bookService.CreateBookAsync("missing-user", input);

            Assert.IsFalse(success);
            Assert.That(reason, Is.EqualTo(UserNotFoundErrorMessage));
        }

        [Test]
        public async Task CreateBookAsync_ShouldFail_WhenGenreNotFound()
        {

            _userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser());

            _genreRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Genre)null);

            var input = new BookCreateInputModel { GenreId = Guid.NewGuid(), PublishedDate = "2024-01-01" };

            var (success, reason) = await _bookService.CreateBookAsync("test-user", input);

            Assert.IsFalse(success);
            Assert.That(reason, Is.EqualTo(InvalidGenreErrorMessage));
        }

        [Test]
        public async Task CreateBookAsync_ShouldFail_WhenPublishedDateInvalid()
        {
            _userManagerMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser());

            _genreRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Genre());

            _authorServiceMock
                .Setup(x => x.GetOrCreateAuthorAsync(It.IsAny<string>()))
                .ReturnsAsync(new Author());

            var input = new BookCreateInputModel
            {
                GenreId = Guid.NewGuid(),
                Author = "Test Author",
                PublishedDate = "invalid-date"
            };

            var (success, reason) = await _bookService.CreateBookAsync("test-user", input);

            Assert.IsFalse(success);
            Assert.That(reason, Is.EqualTo(InvalidBookErrorMessage));
        }

        [Test]
        public async Task GetBookByIdAsync_ShouldReturnBook_WhenBookExists()
        {
            
            var bookId = Guid.NewGuid();
            var expectedBook = new Book { Id = bookId, Title = "Test Book" };

            _bookRepositoryMock
                .Setup(r => r.GetByIdAsync(bookId))
                .ReturnsAsync(expectedBook);

            var result = await _bookService.GetBookByIdAsync(bookId);
    
            Assert.That(result, Is.EqualTo(expectedBook));
            _bookRepositoryMock.Verify(r => r.GetByIdAsync(bookId), Times.Once);
        }

        [Test]
        public async Task GetBookDetailsAsync_ShouldReturnViewModel_WhenBookExists_AndUserIsNull()
        {

            var bookId = Guid.NewGuid();

            var book = new Book
            {
                Id = bookId,
                Title = "Test Book",
                Description = "Desc",
                ImageUrl = "url",
                PublishedDate = new DateTime(2020, 1, 1),
                Author = new Author { Name = "Author Name" },
                Genre = new Genre { Name = "Genre Name" },
                BookCreatorId = "creator-id"
            };

            var pagedReviews = new BookReviewsViewModel
            {
                Reviews = new PagedResult<ReviewDisplayInputModel>
                {
                    Items = new List<ReviewDisplayInputModel>(),
                    PageNumber = 1,
                    PageSize = 5
                }
            };

            _bookRepositoryMock
                .Setup(x => x.GetBookWithDetailsAsync(bookId))
                .ReturnsAsync(book);

            _reviewServiceMock
                .Setup(x => x.GetBookReviewsAsync(bookId, 1, It.IsAny<int>()))
                .ReturnsAsync(pagedReviews);

            var result = await _bookService.GetBookDetailsAsync(bookId, null, 1);

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(book.Id));
            Assert.That(result.AuthorName, Is.EqualTo(book.Author.Name));
            Assert.IsFalse(result.IsApprovedMember);
            Assert.IsFalse(result.HasBorrowedThisBook);
            Assert.IsFalse(result.HasBorrowedAnyBook);
            Assert.That(result.Reviews, Is.EqualTo(pagedReviews));

            _membershipServiceMock.Verify(x => x.GetMembershipByUserIdAsync(It.IsAny<string>()), Times.Never);
            _borrowingRecordServiceMock.Verify(x => x.IsBookBorrowedByMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public void GetBookDetailsAsync_ShouldThrow_WhenBookNotFound()
        {

            var bookId = Guid.NewGuid();

            _bookRepositoryMock
                .Setup(x => x.GetBookWithDetailsAsync(bookId))
                .ReturnsAsync((Book)null);

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _bookService.GetBookDetailsAsync(bookId, null, 1));

            Assert.That(ex.Message, Is.EqualTo(BookNotFoundErrorMessage)); 
        }

        [Test]
        public async Task GetBookDetailsAsync_ShouldSetMemberFlags_WhenUserIsApprovedMember()
        {

            var bookId = Guid.NewGuid();
            var userId = "user-123";
            var memberId = Guid.NewGuid();

            var book = new Book
            {
                Id = bookId,
                Title = "Test Book",
                Description = "Desc",
                ImageUrl = "url",
                PublishedDate = new DateTime(2020, 1, 1),
                Author = new Author { Name = "Author Name" },
                Genre = new Genre { Name = "Genre Name" },
                BookCreatorId = "creator-id"
            };

            var pagedReviews = new BookReviewsViewModel
            {
                Reviews = new PagedResult<ReviewDisplayInputModel>
                {
                    Items = new List<ReviewDisplayInputModel>(),
                    PageNumber = 1,
                    PageSize = 5
                }
            };

            var membership = new Member
            {
                Id = memberId,
                Status = MembershipStatus.Approved
            };

            var memberReview = new ReviewInputModel
            {
                BookId = bookId,
                ReviewId = Guid.NewGuid(),
                Content = "Great book!",
                Rating = 5
            };

            _bookRepositoryMock
                .Setup(x => x.GetBookWithDetailsAsync(bookId))
                .ReturnsAsync(book);

            _reviewServiceMock
                .Setup(x => x.GetBookReviewsAsync(bookId, 1, It.IsAny<int>()))
                .ReturnsAsync(pagedReviews);

            _membershipServiceMock
                .Setup(x => x.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(membership);

            _borrowingRecordServiceMock
                .Setup(x => x.IsBookBorrowedByMemberAsync(memberId, bookId))
                .ReturnsAsync(true);

            _borrowingRecordServiceMock
                .Setup(x => x.HasAnyActiveBorrowAsync(memberId))
                .ReturnsAsync(true);

            _reviewServiceMock
                .Setup(x => x.GetMemberReviewForBookAsync(memberId, bookId))
                .ReturnsAsync(memberReview);

            var result = await _bookService.GetBookDetailsAsync(bookId, userId, 1);

            Assert.IsTrue(result.IsApprovedMember);
            Assert.IsTrue(result.HasBorrowedThisBook);
            Assert.IsTrue(result.HasBorrowedAnyBook);
            Assert.That(result.MemberReview, Is.EqualTo(memberReview));
        }

        [Test]
        public async Task GetBookForDeletingAsync_ShouldReturnModel_WhenUserIsAuthorized()
        {

            var bookId = Guid.NewGuid();
            var userId = "test-user-id";

            var book = new Book
            {
                Id = bookId,
                Title = "Test Title",
                Author = new Author { Name = "Test Author" },
                BookCreatorId = userId
            };

            _bookRepositoryMock
                .Setup(x => x.GetBookWithDetailsAsync(bookId))
                .ReturnsAsync(book);

            var result = await _bookService.GetBookForDeletingAsync(userId, bookId);

            Assert.That(result.Id, Is.EqualTo(bookId));
            Assert.That(result.Title, Is.EqualTo(book.Title));
            Assert.That(result.AuthorName, Is.EqualTo(book.Author.Name));
        }

        [Test]
        public void GetBookForDeletingAsync_ShouldThrowKeyNotFound_WhenBookDoesNotExist()
        {
            
            var bookId = Guid.NewGuid();
            var userId = "test-user-id";

            _bookRepositoryMock
                .Setup(x => x.GetBookWithDetailsAsync(bookId))
                .ReturnsAsync((Book?)null);

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(
                () => _bookService.GetBookForDeletingAsync(userId, bookId));

            Assert.That(ex.Message, Is.EqualTo(BookNotFoundErrorMessage));
        }

        [Test]
        public void GetBookForDeletingAsync_ShouldThrowUnauthorized_WhenUserIsNotCreator()
        {
  
            var bookId = Guid.NewGuid();
            var userId = "test-user-id";

            var book = new Book
            {
                Id = bookId,
                Title = "Test Title",
                Author = new Author { Name = "Test Author" },
                BookCreatorId = "another-user-id"
            };

            _bookRepositoryMock
                .Setup(x => x.GetBookWithDetailsAsync(bookId))
                .ReturnsAsync(book);

            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _bookService.GetBookForDeletingAsync(userId, bookId));

            Assert.That(ex.Message, Is.EqualTo(NotAuthorizedErrorMessage));
        }

        [Test]
        public async Task GetBookForEditingAsync_ShouldReturnModel_WhenUserIsAuthorized()
        {

            var bookId = Guid.NewGuid();
            var userId = "test-user-id";

            var book = new Book
            {
                Id = bookId,
                Title = "Test Title",
                Description = "Test Description",
                ImageUrl = "http://image.url",
                GenreId = Guid.NewGuid(),
                PublishedDate = new DateTime(2024, 1, 1),
                BookCreatorId = userId,
                Author = new Author { Name = "Test Author" }
            };

            _bookRepositoryMock
                .Setup(x => x.GetBookWithDetailsAsync(bookId))
                .ReturnsAsync(book);

            var result = await _bookService.GetBookForEditingAsync(userId, bookId);

            Assert.That(result.Id, Is.EqualTo(book.Id));
            Assert.That(result.Title, Is.EqualTo(book.Title));
            Assert.That(result.Description, Is.EqualTo(book.Description));
            Assert.That(result.ImageUrl, Is.EqualTo(book.ImageUrl));
            Assert.That(result.GenreId, Is.EqualTo(book.GenreId));
            Assert.That(result.PublishedDate, Is.EqualTo(book.PublishedDate.ToString("dd-MM-yyyy")));
            Assert.That(result.Author, Is.EqualTo(book.Author.Name));
        }

        [Test]
        public void GetBookForEditingAsync_ShouldThrowKeyNotFound_WhenBookDoesNotExist()
        {

            var bookId = Guid.NewGuid();
            var userId = "test-user-id";

            _bookRepositoryMock
                .Setup(x => x.GetBookWithDetailsAsync(bookId))
                .ReturnsAsync((Book?)null);

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _bookService.GetBookForEditingAsync(userId, bookId));

            Assert.That(ex.Message, Is.EqualTo(BookNotFoundErrorMessage));
        }

        [Test]
        public void GetBookForEditingAsync_ShouldThrowUnauthorized_WhenUserIsNotCreator()
        {
       
            var bookId = Guid.NewGuid();
            var userId = "test-user-id";

            var book = new Book
            {
                Id = bookId,
                BookCreatorId = "different-user-id",
                Author = new Author { Name = "Test Author" }
            };

            _bookRepositoryMock
                .Setup(x => x.GetBookWithDetailsAsync(bookId))
                .ReturnsAsync(book);

            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _bookService.GetBookForEditingAsync(userId, bookId));

            Assert.That(ex.Message, Is.EqualTo(NotAuthorizedErrorMessage));
        }
    }
}
