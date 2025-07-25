using LibraryManagement.Data.Interfaces;
using LibraryManagement.Data.Models;
using Moq;

namespace LibraryManagement.Services.Core.Tests.Services.GenreServiceTests
{
    [TestFixture]
    public class GenreServiceTests
    {
        private Mock<IGenreRepository> _genreRepositoryMock;
        private GenreService _genreService;

        [SetUp]
        public void SetUp()
        {
            _genreRepositoryMock = new Mock<IGenreRepository>();
            _genreService = new GenreService(_genreRepositoryMock.Object);
        }

        [Test]
        public async Task GetAllAsSelectListAsync_ReturnsCorrectSelectListItems()
        {

            var genres = new List<Genre>
            {
                new Genre { Id = Guid.NewGuid(), Name = "Horror" },
                new Genre { Id = Guid.NewGuid(), Name = "Mystery" }
            };

            _genreRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(genres);

            var result = (await _genreService.GetAllAsSelectListAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Text, Is.EqualTo("Horror"));
            Assert.That(result[0].Value, Is.EqualTo(genres[0].Id.ToString()));
            Assert.That(result[1].Text, Is.EqualTo("Mystery"));
            Assert.That(result[1].Value, Is.EqualTo(genres[1].Id.ToString()));
        }

        [Test]
        public async Task GetAllAsSelectListAsync_WhenNoGenresExist_ReturnsEmptyList()
        {

            _genreRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Genre>());

            var result = await _genreService.GetAllAsSelectListAsync();

            Assert.That(result, Is.Empty);
        }
    }
}
