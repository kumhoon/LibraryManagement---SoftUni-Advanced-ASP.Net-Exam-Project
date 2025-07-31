namespace LibraryManagement.Services.Core.Tests.Controllers.AuthorControllerTests
{
    using LibraryManagement.Services.Common;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.Controllers;
    using LibraryManagement.Web.ViewModels.Author;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.Logging;
    using Moq;
    using static LibraryManagement.GCommon.PagedResultConstants;
    using static LibraryManagement.GCommon.ErrorMessages;

    [TestFixture]
    public class AuthorControllerTests
    {
        private Mock<IAuthorService> _authorServiceMock;
        private Mock<ILogger<AuthorController>> _loggerMock;
        private AuthorController _authorController;
        [SetUp]
        public void SetUp()
        {
            _authorServiceMock = new Mock<IAuthorService>();
            _loggerMock = new Mock<ILogger<AuthorController>>();
            _authorController = new AuthorController(_authorServiceMock.Object, _loggerMock.Object);
            _authorController.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }
        [TearDown]
        public void TearDown() => _authorController.Dispose();

        [Test]
        public async Task Index_WhenServiceReturnsAuthors_ShouldViewWithModelAndViewData()
        {

            var expectedItems = new List<AuthorWithBooksViewModel>
            {
                new AuthorWithBooksViewModel { Name = "Alice" },
                new AuthorWithBooksViewModel { Name = "Bob" }
            };
            var expectedPagedResult = new PagedResult<AuthorWithBooksViewModel>
            {
                Items = expectedItems,
                TotalItems = 2,
                PageNumber = 1,
                PageSize = 5
            };

            const string searchTerm = "A";
            const int pageNumber = 1;
            const int pageSize = 5;

            _authorServiceMock
                .Setup(s => s.GetAuthorsWithBooksAsync(searchTerm, pageNumber, pageSize))
                .ReturnsAsync(expectedPagedResult);

            var result = await _authorController.Index(searchTerm, pageNumber, pageSize);

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;

            Assert.That(view.Model, Is.TypeOf<PagedResult<AuthorWithBooksViewModel>>());
            var model = (PagedResult<AuthorWithBooksViewModel>)view.Model;

            CollectionAssert.AreEqual(expectedItems.Select(a => a.Name), model.Items.Select(a => a.Name));

            Assert.That(model.PageNumber, Is.EqualTo(pageNumber));
            Assert.That(model.PageSize, Is.EqualTo(pageSize));

            Assert.That(_authorController.ViewData["SearchTerm"], Is.EqualTo(searchTerm));
        }

        [Test]
        public async Task Index_WhenServiceThrowsArgumentOutOfRangeException_ShouldRedirectWithDefaultValues()
        {
            const string searchTerm = "A";
            const int invalidPageNumber = -1;
            const int invalidPageSize = 0;

            _authorServiceMock
                .Setup(s => s.GetAuthorsWithBooksAsync(searchTerm, invalidPageNumber, invalidPageSize))
                .ThrowsAsync(new ArgumentOutOfRangeException());

            var result = await _authorController.Index(searchTerm, invalidPageNumber, invalidPageSize);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;

            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            Assert.That(redirectResult.RouteValues["pageNumber"], Is.EqualTo(DefaultPageNumber));
            Assert.That(redirectResult.RouteValues["pageSize"], Is.EqualTo(DefaultPageSize));
            Assert.That(_authorController.TempData["ErrorMessage"], Is.EqualTo(InvalidPaginationValues));
        }

        [Test]
        public async Task Index_WhenServiceThrowsException_ShouldReturnErrorViewAndLogError()
        {
            const string searchTerm = "A";
            const int pageNumber = 1;
            const int pageSize = 5;

            _authorServiceMock
                .Setup(s => s.GetAuthorsWithBooksAsync(searchTerm, pageNumber, pageSize))
                .ThrowsAsync(new Exception());

            var result = await _authorController.Index(searchTerm, pageNumber, pageSize);

            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;

            Assert.That(viewResult.ViewName, Is.EqualTo("Error"));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(LoadingAuthorsErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>())
                );
        }
    }
}
