namespace LibraryManagement.Services.Core.Tests.Services.BookServiceTests
{
    using LibraryManagement.Data;
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using LibraryManagement.Data.Repository;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Book;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [TestFixture]
    public class BookServiceEFCoreTests
    {
        private LibraryManagementDbContext _context;
        private BookService _bookService;

        private Mock<UserManager<IdentityUser>> _userManagerMock;
        private Mock<IGenreService> _genreServiceMock;
        private Mock<IAuthorService> _authorServiceMock;
        private Mock<IGenreRepository> _genreRepositoryMock;
        private Mock<IMembershipService> _membershipServiceMock;
        private Mock<IBorrowingRecordService> _borrowingRecordServiceMock;
        private Mock<IReviewService> _reviewServiceMock;

        [SetUp]
        public async Task SetUp()
        {
            var options = new DbContextOptionsBuilder<LibraryManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LibraryManagementDbContext(options);  

            var genre = new Genre { Id = Guid.NewGuid(), Name = "Science Fiction" };
            var author = new Author { Id = Guid.NewGuid(), Name = "Isaac Asimov" };
            var testUserId = "test-user-id";

            await _context.Genres.AddAsync(genre);
            await _context.Authors.AddAsync(author);

            var books = new List<Book>
            {
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Foundation",
                    Author = author,
                    AuthorId = author.Id,
                    Genre = genre,
                    GenreId = genre.Id,
                    PublishedDate = new DateTime(1951, 1, 1),
                    Description = "Description 1",
                    ImageUrl = "img1.jpg",
                    BookCreatorId = testUserId
                },
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "The Caves of Steel",
                    Author = author,
                    AuthorId = author.Id,
                    Genre = genre,
                    GenreId = genre.Id,
                    PublishedDate = new DateTime(1953, 1, 1),
                    Description = "Description 2",
                    ImageUrl = "img2.jpg",
                    BookCreatorId = testUserId
                },
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "I, Robot",
                    Author = author,
                    AuthorId = author.Id,
                    Genre = genre,
                    GenreId = genre.Id,
                    PublishedDate = new DateTime(1950, 1, 1),
                    Description = "Description 3",
                    ImageUrl = "img3.jpg",
                    BookCreatorId = testUserId
                }
            };

            await _context.Books.AddRangeAsync(books);
            await _context.SaveChangesAsync();

            _userManagerMock = CreateUserManagerMock();
            _genreServiceMock = new Mock<IGenreService>();
            _authorServiceMock = new Mock<IAuthorService>();
            _genreRepositoryMock = new Mock<IGenreRepository>();
            _membershipServiceMock = new Mock<IMembershipService>();
            _borrowingRecordServiceMock = new Mock<IBorrowingRecordService>();
            _reviewServiceMock = new Mock<IReviewService>();

            IBookRepository bookRepository = new BookRepository(_context);

            _bookService = new BookService(
                _userManagerMock.Object,
                bookRepository,
                _genreServiceMock.Object,
                _authorServiceMock.Object,
                _genreRepositoryMock.Object,
                _membershipServiceMock.Object,
                _borrowingRecordServiceMock.Object,
                _reviewServiceMock.Object
            );
        }

        [TearDown]
        public void TearDown() => _context.Dispose();
        private static Mock<UserManager<IdentityUser>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            return new Mock<UserManager<IdentityUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        [Test]
        public async Task GetBookIndexAsync_NoSearchTerm_ReturnsAllBooks()
        {
            var result = await _bookService.GetBookIndexAsync(null);

            Assert.That(result.TotalItems, Is.EqualTo(3));
            Assert.That(result.Items.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task GetBookIndexAsync_WithMatchingSearchTerm_ReturnsFilteredBooks()
        {
            var result = await _bookService.GetBookIndexAsync("Foundation");

            Assert.That(result.TotalItems, Is.EqualTo(1));
            Assert.That(result.Items.First().Title, Is.EqualTo("Foundation"));
        }

        [Test]
        public async Task GetBookIndexAsync_WithNonMatchingSearchTerm_ReturnsEmpty()
        {
            var result = await _bookService.GetBookIndexAsync("Unknown Book");

            Assert.That(result.TotalItems, Is.EqualTo(0));
            Assert.That(result.Items, Is.Empty);
        }

        [Test]
        public async Task GetBookIndexAsync_WithPagination_ReturnsPagedBooks()
        {
            var result = await _bookService.GetBookIndexAsync(null, pageNumber: 1, pageSize: 2);

            Assert.That(result.TotalItems, Is.EqualTo(3));
            Assert.That(result.Items.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetBookIndexAsync_PageNumberTooHigh_ReturnsEmpty()
        {
            var result = await _bookService.GetBookIndexAsync(null, pageNumber: 10, pageSize: 2);

            Assert.That(result.TotalItems, Is.EqualTo(3));
            Assert.That(result.Items.Count(), Is.EqualTo(0));
        }

        [Test]
        public void GetBookIndexAsync_InvalidPageSize_Throws()
        {
            var ex = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await _bookService.GetBookIndexAsync(null, pageNumber: 1, pageSize: 0);
            });

            StringAssert.Contains("Page size", ex.Message);
        }

        [Test]
        public async Task SoftDeleteBookAsync_ValidUserAndBook_MarksBookAsDeleted()
        {

            var userId = "test-user-id";
            var user = new IdentityUser { Id = userId };
            _userManagerMock.Setup(x => x.FindByIdAsync(userId))
                            .ReturnsAsync(user);

            var book = await _context.Books.FirstAsync();

            var inputModel = new BookDeleteInputModel
            {
                Id = book.Id
            };

            await _bookService.SoftDeleteBookAsync(userId, inputModel);

            var updatedBook = await _context.Books.FindAsync(book.Id);
            Assert.IsTrue(updatedBook.IsDeleted);
        }

        [Test]
        public void SoftDeleteBookAsync_BookNotFound_ThrowsKeyNotFoundException()
        {

            var userId = "test-user-id";
            var user = new IdentityUser { Id = userId };
            _userManagerMock.Setup(x => x.FindByIdAsync(userId))
                            .ReturnsAsync(user);

            var inputModel = new BookDeleteInputModel
            {
                Id = Guid.NewGuid() 
            };

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _bookService.SoftDeleteBookAsync(userId, inputModel));
        }

        [Test]
        public void SoftDeleteBookAsync_UserNotFound_ThrowsInvalidOperationException()
        {
            
            var userId = "non-existent-user";
            _userManagerMock.Setup(x => x.FindByIdAsync(userId))
                            .ReturnsAsync((IdentityUser?)null); 

            var book = _context.Books.First();

            var inputModel = new BookDeleteInputModel
            {
                Id = book.Id
            };

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _bookService.SoftDeleteBookAsync(userId, inputModel));
        }

        [Test]
        public void SoftDeleteBookAsync_UnauthorizedUser_ThrowsUnauthorizedAccessException()
        {
            
            var userId = "unauthorized-user";
            var user = new IdentityUser { Id = userId };
            _userManagerMock.Setup(x => x.FindByIdAsync(userId))
                            .ReturnsAsync(user);


            var book = _context.Books.First();

            book.BookCreatorId = "some-other-user";
            _context.Books.Update(book);
            _context.SaveChanges();

            var inputModel = new BookDeleteInputModel
            {
                Id = book.Id
            };

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _bookService.SoftDeleteBookAsync(userId, inputModel));
        }

        [Test]
        public async Task UpdateEditedBookAsync_ValidInput_UpdatesBook()
        {
            var book = await _context.Books.FirstAsync();
            var userId = book.BookCreatorId;

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(new IdentityUser { Id = userId });

            _genreRepositoryMock.Setup(r => r.GetByIdAsync(book.GenreId))
                .ReturnsAsync(await _context.Genres.FirstAsync());

            var input = new BookEditInputModel
            {
                Id = book.Id,
                Title = "Updated Title",
                Author = "Updated Author",
                Description = "Updated Description",
                ImageUrl = "updated.jpg",
                PublishedDate = book.PublishedDate.ToString("dd-MM-yyyy"),
                GenreId = book.GenreId
            };

            await _bookService.UpdateEditedBookAsync(userId, input);
            var updatedBook = await _context.Books.Include(b => b.Author).FirstAsync(b => b.Id == book.Id);

            Assert.That(updatedBook.Title, Is.EqualTo("Updated Title"));
            Assert.That(updatedBook.Author.Name, Is.EqualTo("Updated Author"));
            Assert.That(updatedBook.Description, Is.EqualTo("Updated Description"));
            Assert.That(updatedBook.ImageUrl, Is.EqualTo("updated.jpg"));
        }
        [Test]
        public void UpdateEditedBookAsync_UserNotFound_ThrowsInvalidOperationException()
        {
            var book = _context.Books.First();
            var input = new BookEditInputModel
            {
                Id = book.Id,
                Title = "Title",
                Author = "Author",
                Description = "Desc",
                ImageUrl = "img.jpg",
                PublishedDate = DateTime.UtcNow.ToString("dd-MM-yyyy"),
                GenreId = book.GenreId
            };

            _userManagerMock.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityUser)null);

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() =>
                _bookService.UpdateEditedBookAsync("non-existent-user", input));

            StringAssert.Contains("User not found", ex.Message);
        }

        [Test]
        public void UpdateEditedBookAsync_GenreNotFound_ThrowsInvalidOperationException()
        {
            var book = _context.Books.First();
            var userId = book.BookCreatorId;

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(new IdentityUser { Id = userId });

            _genreRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Genre)null);

            var input = new BookEditInputModel
            {
                Id = book.Id,
                Title = "Title",
                Author = "Author",
                Description = "Desc",
                ImageUrl = "img.jpg",
                PublishedDate = DateTime.UtcNow.ToString("dd-MM-yyyy"),
                GenreId = Guid.NewGuid()
            };

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() =>
                _bookService.UpdateEditedBookAsync(userId, input));

            StringAssert.Contains("Genre not found", ex.Message);
        }

        [Test]
        public void UpdateEditedBookAsync_BookNotFound_ThrowsKeyNotFoundException()
        {
            var userId = "test-user-id";

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(new IdentityUser { Id = userId });

            _genreRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(_context.Genres.First());

            var input = new BookEditInputModel
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Author = "Author",
                Description = "Desc",
                ImageUrl = "img.jpg",
                PublishedDate = DateTime.UtcNow.ToString("dd-MM-yyyy"),
                GenreId = _context.Genres.First().Id
            };

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _bookService.UpdateEditedBookAsync(userId, input));

            StringAssert.Contains("Book not found", ex.Message);
        }
        [Test]
        public void UpdateEditedBookAsync_UserNotCreator_ThrowsUnauthorizedAccessException()
        {
            var book = _context.Books.First();
            var userId = "different-user";

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(new IdentityUser { Id = userId });

            _genreRepositoryMock.Setup(r => r.GetByIdAsync(book.GenreId))
                .ReturnsAsync(_context.Genres.First());

            var input = new BookEditInputModel
            {
                Id = book.Id,
                Title = "Title",
                Author = "Author",
                Description = "Desc",
                ImageUrl = "img.jpg",
                PublishedDate = DateTime.UtcNow.ToString("dd-MM-yyyy"),
                GenreId = book.GenreId
            };

            var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _bookService.UpdateEditedBookAsync(userId, input));

            StringAssert.Contains("You are not authorized to do this action.", ex.Message);
        }

        [Test]
        public void UpdateEditedBookAsync_InvalidDateFormat_ThrowsFormatException()
        {
            var book = _context.Books.First();
            var userId = book.BookCreatorId;

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(new IdentityUser { Id = userId });

            _genreRepositoryMock.Setup(r => r.GetByIdAsync(book.GenreId))
                .ReturnsAsync(_context.Genres.First());

            var input = new BookEditInputModel
            {
                Id = book.Id,
                Title = "Title",
                Author = "Author",
                Description = "Desc",
                ImageUrl = "img.jpg",
                PublishedDate = "invalid-date",
                GenreId = book.GenreId
            };

            var ex = Assert.ThrowsAsync<FormatException>(() =>
                _bookService.UpdateEditedBookAsync(userId, input));

            StringAssert.Contains("Invalid published date format.", ex.Message);
        }
    }
}
