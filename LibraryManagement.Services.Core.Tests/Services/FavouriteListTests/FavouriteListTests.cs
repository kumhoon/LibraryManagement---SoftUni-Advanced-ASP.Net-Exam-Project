namespace LibraryManagement.Services.Core.Tests.Services.FavouriteListTests
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using Moq;

    [TestFixture]
    public class FavouriteListTests
    {
        private Mock<IFavouriteListRepository> _favouriteListRepositoryMock;
        private Mock<IMembershipRepository> _membershipRepositoryMock;
        private FavouriteListService _favouriteListService;

        [SetUp]
        public void SetUp()
        {
            _favouriteListRepositoryMock = new Mock<IFavouriteListRepository>();
            _membershipRepositoryMock = new Mock<IMembershipRepository>();
            _favouriteListService = new FavouriteListService(_favouriteListRepositoryMock.Object, _membershipRepositoryMock.Object);
        }

        [Test]
        public async Task AddToFavouritesAsync_WhenAddSucceeds_ReturnsTrue()
        {
            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _favouriteListRepositoryMock
                .Setup(r => r.AddAsync(memberId, bookId))
                .ReturnsAsync(true);

            var result = await _favouriteListService.AddToFavouritesAsync(memberId, bookId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task AddToFavouritesAsync_WhenAddFails_ReturnsFalse()
        {

            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _favouriteListRepositoryMock
                .Setup(r => r.AddAsync(memberId, bookId))
                .ReturnsAsync(false);

            var result = await _favouriteListService.AddToFavouritesAsync(memberId, bookId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task RemoveFromFavouritesAsync_WhenRemoveSucceeds_ReturnsTrue()
        {

            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _favouriteListRepositoryMock
                .Setup(r => r.RemoveAsync(memberId, bookId))
                .ReturnsAsync(true);

            var result = await _favouriteListService.RemoveFromFavouritesAsync(memberId, bookId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task RemoveFromFavouritesAsync_WhenRemoveFails_ReturnsFalse()
        {

            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _favouriteListRepositoryMock
                .Setup(r => r.RemoveAsync(memberId, bookId))
                .ReturnsAsync(false);

            var result = await _favouriteListService.RemoveFromFavouritesAsync(memberId, bookId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task IsBookFavouriteAsync_WhenBookIsFavourite_ReturnsTrue()
        {

            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _favouriteListRepositoryMock
                .Setup(r => r.ExistsAsync(memberId, bookId))
                .ReturnsAsync(true);

            var result = await _favouriteListService.IsBookFavouriteAsync(memberId, bookId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsBookFavouriteAsync_WhenBookIsNotFavourite_ReturnsFalse()
        {

            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            _favouriteListRepositoryMock
                .Setup(r => r.ExistsAsync(memberId, bookId))
                .ReturnsAsync(false);

            var result = await _favouriteListService.IsBookFavouriteAsync(memberId, bookId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetFavouriteBooksAsync_WithValidMemberId_ReturnsMappedBooks()
        {
            var memberId = Guid.NewGuid();
            var books = new List<Book>
            {
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Book One",
                    Author = new Author { Name = "Author One" },
                    Genre = new Genre { Name = "Genre One" },
                    PublishedDate = new DateTime(2021, 1, 1),
                    ImageUrl = "book1.jpg"
                },
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Book Two",
                    Author = new Author { Name = "Author Two" },
                    Genre = new Genre { Name = "Genre Two" },
                    PublishedDate = new DateTime(2022, 2, 2),
                    ImageUrl = "book2.jpg"
                }
            };

            _favouriteListRepositoryMock
                .Setup(r => r.GetFavouriteBooksAsync(memberId))
                .ReturnsAsync(books);

            var result = (await _favouriteListService.GetFavouriteBooksAsync(memberId)).ToList();

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Title, Is.EqualTo("Book One"));
            Assert.That(result[0].AuthorName, Is.EqualTo("Author One"));
            Assert.That(result[0].Genre, Is.EqualTo("Genre One"));
            Assert.That(result[1].Title, Is.EqualTo("Book Two"));
        }

        [Test]
        public async Task GetFavouriteBooksAsync_BookHasNullAuthorOrGenre_UsesFallbackValues()
        {
            // Arrange
            var memberId = Guid.NewGuid();
            var books = new List<Book>
            {
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Orphan Book",
                    Author = null,
                    Genre = null,
                    PublishedDate = DateTime.UtcNow,
                    ImageUrl = "noauthor.jpg"
                }
            };

            _favouriteListRepositoryMock
                .Setup(r => r.GetFavouriteBooksAsync(memberId))
                .ReturnsAsync(books);

            // Act
            var result = (await _favouriteListService.GetFavouriteBooksAsync(memberId)).ToList();

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].AuthorName, Is.EqualTo("Unknown Author"));
            Assert.That(result[0].Genre, Is.EqualTo("Unknown Genre"));
        }
    }
}

