namespace LibraryManagement.Services.Core.Tests.Services.AuthorServiceTests
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using Moq;

    [TestFixture]
    public class AuthorServiceTests
    {
        private Mock<IAuthorRepository> _authorRepositoryMock;
        private AuthorService _authorService;

        [SetUp]
        public void SetUp()
        {
            _authorRepositoryMock = new Mock<IAuthorRepository>();
            _authorService = new AuthorService(_authorRepositoryMock.Object);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void GetOrCreateAuthorAsync_InvalidName_ThrowsArgumentException(string? invalidName)
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(() =>
                _authorService.GetOrCreateAuthorAsync(invalidName));

            Assert.That(ex!.ParamName, Is.EqualTo("name"));
            StringAssert.Contains("Author name must be between 1 and 50 characters", ex.Message); 
        }

        [Test]
        public async Task GetOrCreateAuthorAsync_AuthorExists_ReturnsExistingAuthor()
        {
            
            var existingAuthor = new Author
            {
                Id = Guid.NewGuid(),
                Name = "Isaac Asimov"
            };

            _authorRepositoryMock
                .Setup(r => r.GetByNameAsync("Isaac Asimov"))
                .ReturnsAsync(existingAuthor);

            var result = await _authorService.GetOrCreateAuthorAsync("  Isaac Asimov  ");

            Assert.That(result, Is.EqualTo(existingAuthor));
            _authorRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Author>()), Times.Never);
            _authorRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task GetOrCreateAuthorAsync_AuthorDoesNotExist_CreatesAndReturnsAuthor()
        {

            _authorRepositoryMock
                .Setup(r => r.GetByNameAsync("Arthur C. Clarke"))
                .ReturnsAsync((Author?)null);

            Author? addedAuthor = null;

            _authorRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Author>()))
                .Callback<Author>(a => addedAuthor = a)
                .Returns(Task.CompletedTask);

            _authorRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _authorService.GetOrCreateAuthorAsync(" Arthur C. Clarke ");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result.Name, Is.EqualTo("Arthur C. Clarke"));

            Assert.That(addedAuthor, Is.Not.Null);
            Assert.That(addedAuthor!.Name, Is.EqualTo("Arthur C. Clarke"));

            _authorRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Author>()), Times.Once);
            _authorRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}

