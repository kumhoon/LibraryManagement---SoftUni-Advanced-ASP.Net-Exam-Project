namespace LibraryManagement.Services.Core.Tests.Controllers.BookControllerTests
{
    using LibraryManagement.Services.Common;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.Controllers;
    using LibraryManagement.Web.ViewModels.Book;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System.Security.Claims;
    using static LibraryManagement.GCommon.ErrorMessages;
    using static LibraryManagement.GCommon.PagedResultConstants;

    [TestFixture]
    public class BookControllerTests
    {
        private Mock<IBookService> _bookServiceMock;
        private Mock<IGenreService> _genreServiceMock;
        private Mock<ILogger<BookController>> _loggerMock;
        private BookController _bookController;
        [SetUp]
        public void SetUp()
        {
            _bookServiceMock = new Mock<IBookService>();
            _loggerMock = new Mock<ILogger<BookController>>();
            _genreServiceMock = new Mock<IGenreService>();
            _bookController = new BookController(_bookServiceMock.Object, _genreServiceMock.Object, _loggerMock.Object);
            _bookController.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }
        [TearDown]
        public void TearDown() => _bookController.Dispose();

        private void FakeUserId(string userId)
        {
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "mock"));
            _bookController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Test]
        public async Task Index_WhenServiceReturnsBooks_ShouldReturnViewWithModelAndViewData()
        {
            var expectedItems = new List<BookIndexViewModel>
            {
                new BookIndexViewModel { Title = "Book 1", AuthorName = "Author 1" },
                new BookIndexViewModel { Title = "Book 2", AuthorName = "Author 2" }
            };

            var expectedPagedResult = new PagedResult<BookIndexViewModel>
            {
                Items = expectedItems,
                TotalItems = 2,
                PageNumber = 1,
                PageSize = 5
            };

            const string searchTerm = "B";
            const int pageNumber = 1;
            const int pageSize = 5;

            _bookServiceMock
                .Setup(s => s.GetBookIndexAsync(searchTerm, pageNumber, pageSize))
                .ReturnsAsync(expectedPagedResult);

            var result = await _bookController.Index(searchTerm, pageNumber, pageSize);

            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;

            Assert.That(viewResult.Model, Is.AssignableTo<PagedResult<BookIndexViewModel>>());
            var model = (PagedResult<BookIndexViewModel>)viewResult.Model;

            Assert.That(model.Items.Count(), Is.EqualTo(2));
            Assert.That(model.Items.First().Title, Is.EqualTo(expectedItems.First().Title));
            Assert.That(model.Items.First().AuthorName, Is.EqualTo(expectedItems.First().AuthorName));
            Assert.That(model.TotalItems, Is.EqualTo(expectedPagedResult.TotalItems));
            Assert.That(model.PageNumber, Is.EqualTo(expectedPagedResult.PageNumber));
            Assert.That(model.PageSize, Is.EqualTo(expectedPagedResult.PageSize));
            Assert.That(viewResult.ViewData["SearchTerm"], Is.EqualTo(searchTerm));
        }

        [Test]
        public async Task Index_WhenArgumentOutOfRangeExceptionThrown_ShouldReturnWarningViewAndLogError()
        {
            const string searchTerm = "B";
            const int pageNumber = 0;
            const int pageSize = 5;

            _bookServiceMock
                .Setup(s => s.GetBookIndexAsync(searchTerm, pageNumber, pageSize))
                .ThrowsAsync(new ArgumentOutOfRangeException());

            var result = await _bookController.Index(searchTerm, pageNumber, pageSize);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;

            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            Assert.That(redirectResult.RouteValues["pageNumber"], Is.EqualTo(DefaultPageNumber));
            Assert.That(redirectResult.RouteValues["pageSize"], Is.EqualTo(DefaultPageSize));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(InvalidPaginationValues)),
                    It.IsAny<ArgumentOutOfRangeException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>())
                );
        }

        [Test]
        public async Task Index_WhenExceptionThrown_ShouldReturnErrorViewAndLogError()
        {
            const string searchTerm = "B";
            const int pageNumber = 1;
            const int pageSize = 5;

            _bookServiceMock
                .Setup(s => s.GetBookIndexAsync(searchTerm, pageNumber, pageSize))
                .ThrowsAsync(new Exception());

            var result = await _bookController.Index(searchTerm, pageNumber, pageSize);

            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;

            Assert.That(viewResult.ViewName, Is.EqualTo("Error"));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(UnexpectedErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>())
                );
        }

        [Test]
        public async Task Details_WhenBookExists_ShouldReturnViewWithModel()
        {
        
            var bookId = Guid.NewGuid();
            var userId = "user123";
            var expectedModel = new BookDetailsViewModel { Id = bookId };

            _bookServiceMock
                .Setup(s => s.GetBookDetailsAsync(bookId, userId, DefaultPageNumber))
                .ReturnsAsync(expectedModel);


            FakeUserId("user123");

            var result = await _bookController.Details(bookId);


            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;

            Assert.That(view.Model, Is.TypeOf<BookDetailsViewModel>());
            var model = (BookDetailsViewModel)view.Model;
            Assert.That(model.Id, Is.EqualTo(bookId));
        }

        [Test]
        public async Task Details_WhenServiceReturnsNull_ShouldReturnNotFound()
        {
            var bookId = Guid.NewGuid();

            _bookServiceMock
                .Setup(s => s.GetBookDetailsAsync(bookId, It.IsAny<string>(), DefaultPageNumber))
                .ReturnsAsync((BookDetailsViewModel?)null);

            var result = await _bookController.Details(bookId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task Details_WhenServiceThrowsGeneralException_ShouldRedirectAndLogError()
        {
            var bookId = Guid.NewGuid();

            _bookServiceMock
                .Setup(s => s.GetBookDetailsAsync(bookId, It.IsAny<string>(), DefaultPageNumber))
                .ThrowsAsync(new Exception("db failed"));

            var result = await _bookController.Details(bookId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;

            Assert.That(redirect.ActionName, Is.EqualTo(nameof(BookController.Index)));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(UnexpectedErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task Create_WhenGenresReturned_ShouldReturnViewWithInputModel()
        {

            var genres = new List<SelectListItem>
            {
                new SelectListItem { Text = "Fiction", Value = "1" },
                new SelectListItem { Text = "Sci-Fi",  Value = "2" }
            };

            _genreServiceMock
                .Setup(s => s.GetAllAsSelectListAsync())
                .ReturnsAsync(genres);

            var result = await _bookController.Create();

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;

            Assert.That(view.Model, Is.TypeOf<BookCreateInputModel>());
            var model = (BookCreateInputModel)view.Model;

            Assert.That(model.Genres.Count(), Is.EqualTo(2));
            Assert.That(model.Genres.First().Text, Is.EqualTo("Fiction"));
        }

        [Test]
        public async Task Create_WhenServiceThrowsException_ShouldRedirectAndLogError()
        {

            _genreServiceMock
                .Setup(s => s.GetAllAsSelectListAsync())
                .ThrowsAsync(new Exception("DB down"));

            var result = await _bookController.Create();

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(BookController.Index)));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(UnexpectedErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task Create_WhenModelStateInvalid_ShouldReturnViewWithGenres()
        {

            var inputModel = new BookCreateInputModel();
            _bookController.ModelState.AddModelError("Title", "Required");

            var genres = new List<SelectListItem>
            {
                new SelectListItem { Text = "Drama", Value = "1" }
            };

            _genreServiceMock
                .Setup(s => s.GetAllAsSelectListAsync())
                .ReturnsAsync(genres);

            var result = await _bookController.Create(inputModel);

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;

            Assert.That(view.Model, Is.TypeOf<BookCreateInputModel>());
            var model = (BookCreateInputModel)view.Model;

            Assert.That(model.Genres.Count(), Is.EqualTo(1));
            Assert.That(model.Genres.First().Text, Is.EqualTo("Drama"));
        }

        [Test]
        public async Task Create_WhenServiceFails_ShouldAddModelErrorAndReturnViewWithGenres()
        {

            var inputModel = new BookCreateInputModel { Title = "Test" };
            const string reason = "Duplicate book";

            _bookServiceMock
                .Setup(s => s.CreateBookAsync(It.IsAny<string>(), inputModel))
                .ReturnsAsync((false, reason));

            _genreServiceMock
                .Setup(s => s.GetAllAsSelectListAsync())
                .ReturnsAsync(new List<SelectListItem>
                {
                    new SelectListItem { Text = "Drama", Value = "1" }
                });

            FakeUserId("admin123");

            var result = await _bookController.Create(inputModel);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.That(_bookController.ModelState[string.Empty]!.Errors.First().ErrorMessage,
                        Is.EqualTo(reason));
        }

        [Test]
        public async Task Create_WhenServiceSucceeds_ShouldRedirectToIndex()
        {

            var inputModel = new BookCreateInputModel { Title = "New Book" };

            _bookServiceMock
                .Setup(s => s.CreateBookAsync(It.IsAny<string>(), inputModel))
                .ReturnsAsync((true, null));

            FakeUserId("admin123");

            var result = await _bookController.Create(inputModel);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(BookController.Index)));
        }

        [Test]
        public async Task CreatePost_WhenServiceThrowsException_ShouldRedirectAndLogError()
        {

            var inputModel = new BookCreateInputModel { Title = "Book" };

            _bookServiceMock
                .Setup(s => s.CreateBookAsync(It.IsAny<string>(), inputModel))
                .ThrowsAsync(new Exception("DB failure"));

            FakeUserId("admin123");

            var result = await _bookController.Create(inputModel);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(BookController.Index)));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(UnexpectedErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task Edit_WhenIdIsNull_ShouldRedirectToIndex()
        {
            var result = await _bookController.Edit((Guid?)null);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(BookController.Index)));
        }

        [Test]
        public async Task Edit_WhenBookFound_ShouldReturnViewWithModelAndGenres()
        {

            var bookId = Guid.NewGuid();
            var userId = "admin123";
            var editModel = new BookEditInputModel { Title = "Editable Book" };
            var genres = new List<SelectListItem>
            {
                new SelectListItem { Text = "Drama", Value = "1" }
            };

            _bookServiceMock
                .Setup(s => s.GetBookForEditingAsync(userId, bookId))
                .ReturnsAsync(editModel);

            _genreServiceMock
                .Setup(s => s.GetAllAsSelectListAsync())
                .ReturnsAsync(genres);

            FakeUserId(userId);

            var result = await _bookController.Edit(bookId);

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;

            Assert.That(view.Model, Is.TypeOf<BookEditInputModel>());
            var model = (BookEditInputModel)view.Model;

            Assert.That(model.Title, Is.EqualTo("Editable Book"));
            Assert.That(model.Genres.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task Edit_WhenBookNotFound_ShouldReturnNotFound()
        {

            var bookId = Guid.NewGuid();
            var userId = "admin123";

            _bookServiceMock
                .Setup(s => s.GetBookForEditingAsync(userId, bookId))
                .ThrowsAsync(new KeyNotFoundException());

            FakeUserId(userId);

            var result = await _bookController.Edit(bookId);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_WhenUnauthorized_ShouldReturnForbid()
        {

            var bookId = Guid.NewGuid();
            var userId = "admin123";

            _bookServiceMock
                .Setup(s => s.GetBookForEditingAsync(userId, bookId))
                .ThrowsAsync(new UnauthorizedAccessException());

            FakeUserId(userId);

            var result = await _bookController.Edit(bookId);

            Assert.IsInstanceOf<ForbidResult>(result);
        }

        [Test]
        public async Task Edit_WhenGeneralException_ShouldLogErrorAndReturnErrorView()
        {

            var bookId = Guid.NewGuid();
            var userId = "admin123";

            _bookServiceMock
                .Setup(s => s.GetBookForEditingAsync(userId, bookId))
                .ThrowsAsync(new Exception("Unexpected"));

            FakeUserId(userId);

            var result = await _bookController.Edit(bookId);

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;
            Assert.That(view.ViewName, Is.EqualTo("Error"));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(BookEditErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task EditPost_WhenModelStateInvalid_ShouldReturnViewWithGenres()
        {

            var model = new BookEditInputModel();
            _bookController.ModelState.AddModelError("Title", "Required");

            _genreServiceMock
                .Setup(s => s.GetAllAsSelectListAsync())
                .ReturnsAsync(new List<SelectListItem>
                {
            new SelectListItem { Text = "Drama", Value = "1" }
                });

            var result = await _bookController.Edit(model);

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;
            var returnedModel = (BookEditInputModel)view.Model;

            Assert.That(returnedModel.Genres.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task EditPost_WhenUpdateSuccessful_ShouldRedirectToDetails()
        {

            var model = new BookEditInputModel { Id = Guid.NewGuid(), Title = "Updated" };

            _bookServiceMock
                .Setup(s => s.UpdateEditedBookAsync(It.IsAny<string>(), model))
                .Returns(Task.CompletedTask);

            FakeUserId("admin123");

            var result = await _bookController.Edit(model);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(BookController.Details)));
            Assert.That(redirect.RouteValues!["id"], Is.EqualTo(model.Id));
        }

        [Test]
        public async Task EditPost_WhenFormatException_ShouldAddModelErrorAndReturnView()
        {
            var model = new BookEditInputModel { Id = Guid.NewGuid() };

            _bookServiceMock
                .Setup(s => s.UpdateEditedBookAsync(It.IsAny<string>(), model))
                .ThrowsAsync(new FormatException());

            _genreServiceMock
                .Setup(s => s.GetAllAsSelectListAsync())
                .ReturnsAsync(new List<SelectListItem>());

            FakeUserId("admin123");

            var result = await _bookController.Edit(model);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.That(_bookController.ModelState[nameof(model.PublishedDate)]?.Errors.First().ErrorMessage,
                        Is.EqualTo(InvalidPublishedDateFormatErrorMessage));
        }

        [Test]
        public async Task EditPost_WhenKeyNotFound_ShouldReturnNotFound()
        {
            var model = new BookEditInputModel { Id = Guid.NewGuid() };

            _bookServiceMock
                .Setup(s => s.UpdateEditedBookAsync(It.IsAny<string>(), model))
                .ThrowsAsync(new KeyNotFoundException());

            FakeUserId("admin123");

            var result = await _bookController.Edit(model);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task EditPost_WhenUnauthorized_ShouldReturnForbid()
        {
            var model = new BookEditInputModel { Id = Guid.NewGuid() };

            _bookServiceMock
                .Setup(s => s.UpdateEditedBookAsync(It.IsAny<string>(), model))
                .ThrowsAsync(new UnauthorizedAccessException());

            FakeUserId("admin123");

            var result = await _bookController.Edit(model);

            Assert.IsInstanceOf<ForbidResult>(result);
        }

        [Test]
        public async Task EditPost_WhenGeneralException_ShouldLogErrorAddModelErrorAndReturnView()
        {
            var model = new BookEditInputModel { Id = Guid.NewGuid() };

            _bookServiceMock
                .Setup(s => s.UpdateEditedBookAsync(It.IsAny<string>(), model))
                .ThrowsAsync(new Exception("DB failure"));

            _genreServiceMock
                .Setup(s => s.GetAllAsSelectListAsync())
                .ReturnsAsync(new List<SelectListItem>());

            FakeUserId("admin123");

            var result = await _bookController.Edit(model);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.That(_bookController.ModelState[string.Empty]?.Errors.First().ErrorMessage,
                        Is.EqualTo(UnexpectedErrorMessage));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(BookUpdateErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task Delete_WhenBookFound_ShouldReturnViewWithModel()
        {

            var bookId = Guid.NewGuid();
            var userId = "admin123";
            var bookModel = new BookDeleteInputModel { Id = bookId, Title = "Book to Delete" };

            _bookServiceMock
                .Setup(s => s.GetBookForDeletingAsync(userId, bookId))
                .ReturnsAsync(bookModel);

            FakeUserId(userId);

            var result = await _bookController.Delete(bookId);

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;
            var model = (BookDeleteInputModel)view.Model;

            Assert.That(model.Id, Is.EqualTo(bookId));
            Assert.That(model.Title, Is.EqualTo("Book to Delete"));
        }

        [Test]
        public async Task Delete_WhenBookNotFound_ShouldReturnNotFound()
        {
            var bookId = Guid.NewGuid();
            var userId = "admin123";

            _bookServiceMock
                .Setup(s => s.GetBookForDeletingAsync(userId, bookId))
                .ThrowsAsync(new KeyNotFoundException());

            FakeUserId(userId);

            var result = await _bookController.Delete(bookId);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_WhenUnauthorized_ShouldReturnForbid()
        {
            var bookId = Guid.NewGuid();
            var userId = "admin123";

            _bookServiceMock
                .Setup(s => s.GetBookForDeletingAsync(userId, bookId))
                .ThrowsAsync(new UnauthorizedAccessException());

            FakeUserId(userId);

            var result = await _bookController.Delete(bookId);

            Assert.IsInstanceOf<ForbidResult>(result);
        }

        [Test]
        public async Task Delete_WhenGeneralException_ShouldLogErrorAndReturnErrorView()
        {
            var bookId = Guid.NewGuid();
            var userId = "admin123";

            _bookServiceMock
                .Setup(s => s.GetBookForDeletingAsync(userId, bookId))
                .ThrowsAsync(new Exception("Unexpected failure"));

            FakeUserId(userId);

            var result = await _bookController.Delete(bookId);

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;
            Assert.That(view.ViewName, Is.EqualTo("Error"));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(UnexpectedErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task ConfirmDelete_WhenModelStateInvalid_ShouldAddModelErrorAndReturnView()
        {

            var inputModel = new BookDeleteInputModel { Id = Guid.NewGuid() };
            _bookController.ModelState.AddModelError("Dummy", "Invalid");

            var result = await _bookController.ConfirmDelete(inputModel);

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;
            var model = (BookDeleteInputModel)view.Model;

            Assert.That(_bookController.ModelState[string.Empty]!.Errors.First().ErrorMessage,
                        Is.EqualTo(InvalidDataErrorMessage));
            Assert.That(model, Is.SameAs(inputModel));
        }

        [Test]
        public async Task ConfirmDelete_WhenServiceSucceeds_ShouldRedirectToIndex()
        {

            var inputModel = new BookDeleteInputModel { Id = Guid.NewGuid() };
            FakeUserId("admin123");

            _bookServiceMock
                .Setup(s => s.SoftDeleteBookAsync("admin123", inputModel))
                .Returns(Task.CompletedTask);

            var result = await _bookController.ConfirmDelete(inputModel);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(BookController.Index)));
        }

        [Test]
        public async Task ConfirmDelete_WhenServiceThrowsKeyNotFound_ShouldReturnNotFound()
        {

            var inputModel = new BookDeleteInputModel { Id = Guid.NewGuid() };
            FakeUserId("admin123");

            _bookServiceMock
                .Setup(s => s.SoftDeleteBookAsync("admin123", inputModel))
                .ThrowsAsync(new KeyNotFoundException());

            var result = await _bookController.ConfirmDelete(inputModel);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task ConfirmDelete_WhenServiceThrowsUnauthorized_ShouldReturnForbid()
        {

            var inputModel = new BookDeleteInputModel { Id = Guid.NewGuid() };
            FakeUserId("admin123");

            _bookServiceMock
                .Setup(s => s.SoftDeleteBookAsync("admin123", inputModel))
                .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _bookController.ConfirmDelete(inputModel);

            Assert.IsInstanceOf<ForbidResult>(result);
        }

        [Test]
        public async Task ConfirmDelete_WhenGeneralException_ShouldLogErrorAddModelErrorAndReturnView()
        {

            var inputModel = new BookDeleteInputModel { Id = Guid.NewGuid() };
            FakeUserId("admin123");

            _bookServiceMock
                .Setup(s => s.SoftDeleteBookAsync("admin123", inputModel))
                .ThrowsAsync(new Exception("DB failure"));

            var result = await _bookController.ConfirmDelete(inputModel);

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;
            var model = (BookDeleteInputModel)view.Model;

            Assert.That(_bookController.ModelState[string.Empty]!.Errors.First().ErrorMessage,
                        Is.EqualTo(UnexpectedErrorMessage));
            Assert.That(model, Is.SameAs(inputModel));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(BookDeleteErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }
    }
}
