namespace LibraryManagement.Services.Core.Tests.Services.AuthorServiceTests
{
    using LibraryManagement.Data;
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using LibraryManagement.Data.Repository;
    using Microsoft.EntityFrameworkCore;

    [TestFixture]
    public class AuthorServiceEFCoreTests
    {
        private LibraryManagementDbContext _context;
        private AuthorService _authorService;

        [SetUp]
        public async Task Setup() 
        {
            var options = new DbContextOptionsBuilder<LibraryManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LibraryManagementDbContext(options);

            var author1 = new Author
            {
                Id = Guid.NewGuid(),
                Name = "Isaac Asimov",
                Books = new List<Book>
                {
                new Book { Title = "Foundation", IsDeleted = false, Description = "Something", BookCreatorId = "adm"},
                new Book { Title = "I, Robot", IsDeleted = false, Description = "Other thing", BookCreatorId = "adm" }
                }
            };

            var author2 = new Author
            {
                Id = Guid.NewGuid(),
                Name = "Arthur C. Clarke",
                Books = new List<Book>
                {
                new Book { Title = "2001: A Space Odyssey", IsDeleted = false, Description = "Another thing", BookCreatorId = "us1" }
                }
            };

            var author3 = new Author
            {
                Id = Guid.NewGuid(),
                Name = "Deleted Author",
                Books = new List<Book>
                {
                new Book { Title = "Ghost Book", IsDeleted = true, Description = "Something else", BookCreatorId = "us2" }
                }
            };

            await _context.Authors.AddRangeAsync(author1, author2, author3);
            await _context.SaveChangesAsync();

            IAuthorRepository authorRepository = new AuthorRepository(_context);
            _authorService = new AuthorService(authorRepository);
        }
        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public async Task GetAuthorsWithBooksAsync_ReturnsOnlyAuthorsWithNonDeletedBooks()
        {
            var result = await _authorService.GetAuthorsWithBooksAsync(null, 1, 5);

            Assert.That(result.Items.Count(), Is.EqualTo(2));
            Assert.IsTrue(result.Items.All(a => a.Books.Count() > 0));
            Assert.IsFalse(result.Items.Any(a => a.Name == "Deleted Author"));
        }

        [Test]
        public async Task GetAuthorsWithBooksAsync_WithSearchTerm_FiltersCorrectly()
        {
            var result = await _authorService.GetAuthorsWithBooksAsync("Isaac", 1, 5);

            Assert.That(result.Items.Count(), Is.EqualTo(1));
            Assert.That(result.Items.First().Name, Is.EqualTo("Isaac Asimov"));
        }

        [Test]
        public async Task GetAuthorsWithBooksAsync_Pagination_WorksCorrectly()
        {
            var result = await _authorService.GetAuthorsWithBooksAsync(null, 1, 1);

            Assert.That(result.Items.Count(), Is.EqualTo(1));

            Assert.That(result.PageNumber, Is.EqualTo(1));
            Assert.That(result.PageSize, Is.EqualTo(1));
            Assert.That(result.TotalItems, Is.EqualTo(2)); 
        }

        [TestCase(0, 10)]
        [TestCase(1, 0)]
        public void GetAuthorsWithBooksAsync_InvalidPagination_ThrowsArgumentException(int pageNumber, int pageSize)
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                _authorService.GetAuthorsWithBooksAsync(null, pageNumber, pageSize));
        }

        [Test]
        public async Task GetAuthorsWithBooksAsync_AuthorsWithNoBooksOrOnlyDeletedBooks_AreExcluded()
        {
            
            var authorWithNoBooks = new Author
            {
                Id = Guid.NewGuid(),
                Name = "No Books Author",
                Books = new List<Book>() 
            };

            await _context.Authors.AddAsync(authorWithNoBooks);
            await _context.SaveChangesAsync();

            var result = await _authorService.GetAuthorsWithBooksAsync(null, 1, 10);
            
            Assert.IsFalse(result.Items.Any(a => a.Name == "No Books Author"));
        }
    }
}

