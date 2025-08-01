namespace LibraryManagement.Services.Core.Tests.Controllers.FavouriteListControllerTests
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.Controllers;
    using LibraryManagement.Web.ViewModels.Book;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System.Security.Claims;
    using static LibraryManagement.GCommon.ErrorMessages;
    using static LibraryManagement.GCommon.Messages.FavouriteListMessages;

    [TestFixture]
    public class FavouriteListControllerTests
    {
        private Mock<IFavouriteListService> _favouriteServiceMock;
        private Mock<IMembershipService> _membershipServiceMock;
        private Mock<ILogger<FavouriteListController>> _loggerMock;
        private FavouriteListController _controller;

        [SetUp]
        public void SetUp()
        {
            _favouriteServiceMock = new Mock<IFavouriteListService>();
            _membershipServiceMock = new Mock<IMembershipService>();
            _loggerMock = new Mock<ILogger<FavouriteListController>>();

            _controller = new FavouriteListController(
                _favouriteServiceMock.Object,
                _membershipServiceMock.Object,
                _loggerMock.Object
            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            _controller.TempData = new TempDataDictionary(_controller.ControllerContext.HttpContext,Mock.Of<ITempDataProvider>());
        }

        [TearDown]
        public void TearDown() => _controller.Dispose();

        private void FakeUser(string userId)
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "mock"));
            _controller.ControllerContext.HttpContext.User = principal;
        }

        [Test]
        public async Task Index_NoUser_ShouldReturnUnauthorized()
        { 
            var result = await _controller.Index();
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Index_UserWithoutMembership_ShouldRedirectToBookIndex()
        {

            const string userId = "user123";
            FakeUser(userId);
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync((Member)null!);

            var result = await _controller.Index();

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Book"));
        }

        [Test]
        public async Task Index_WithMembership_ShouldReturnViewWithFavouriteBooks()
        {

            const string userId = "user123";
            FakeUser(userId);

            var member = new Member { Id = Guid.NewGuid() };
            var favs = new List<BookIndexViewModel>
            {
                new BookIndexViewModel { Id = Guid.NewGuid(), Title = "Test Book" }
            };

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(member);

            _favouriteServiceMock
                .Setup(s => s.GetFavouriteBooksAsync(member.Id))
                .ReturnsAsync(favs);

            var result = await _controller.Index();

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var view = (ViewResult)result;
            Assert.That(view.Model, Is.AssignableTo<IEnumerable<BookIndexViewModel>>());

            var model = (IEnumerable<BookIndexViewModel>)view.Model;
            Assert.That(model, Has.Exactly(1).Items);
            Assert.That(model.First().Title, Is.EqualTo("Test Book"));
        }

        [Test]
        public async Task Index_WhenExceptionThrown_ShouldLogErrorAndReturnErrorView()
        {

            const string userId = "user123";
            FakeUser(userId);

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ThrowsAsync(new Exception("fail"));

            var result = await _controller.Index();

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var view = (ViewResult)result;
            Assert.That(view.ViewName, Is.EqualTo("Error"));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(FavouriteListErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task Add_NoUser_ShouldReturnUnauthorized()
        {
            var result = await _controller.Add(Guid.NewGuid());
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Add_NoMembership_ShouldReturnUnauthorized()
        {
            const string userId = "user1";
            FakeUser(userId);
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync((Member)null!);

            var result = await _controller.Add(Guid.NewGuid());
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Add_WhenAdded_ShouldRedirectWithSuccessMessage()
        {
            const string userId = "user1";
            var bookId = Guid.NewGuid();
            var member = new Member { Id = Guid.NewGuid() };
            FakeUser(userId);

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(member);
            _favouriteServiceMock
                .Setup(s => s.AddToFavouritesAsync(member.Id, bookId))
                .ReturnsAsync(true);

            var result = await _controller.Add(bookId);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            Assert.That(_controller.TempData.ContainsKey("SuccessMessage"), Is.True);
            Assert.That(_controller.TempData["SuccessMessage"], Is.EqualTo(BookAddedToFavourites));
        }

        [Test]
        public async Task Add_WhenAlreadyInFavourites_ShouldRedirectWithErrorMessage()
        {
            const string userId = "user1";
            var bookId = Guid.NewGuid();
            var member = new Member { Id = Guid.NewGuid() };
            FakeUser(userId);

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(member);
            _favouriteServiceMock
                .Setup(s => s.AddToFavouritesAsync(member.Id, bookId))
                .ReturnsAsync(false);

            var result = await _controller.Add(bookId);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            Assert.That(_controller.TempData.ContainsKey("ErrorMessage"), Is.True);
            Assert.That(_controller.TempData["ErrorMessage"], Is.EqualTo(BookAlreadyInFavourites));
        }

        [Test]
        public async Task Add_WhenKeyNotFound_ShouldReturnNotFound()
        {
            const string userId = "user1";
            var bookId = Guid.NewGuid();
            var member = new Member { Id = Guid.NewGuid() };
            FakeUser(userId);

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(member);
            _favouriteServiceMock
                .Setup(s => s.AddToFavouritesAsync(member.Id, bookId))
                .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.Add(bookId);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Add_WhenExceptionThrown_ShouldLogErrorAndRedirectWithUnexpectedError()
        {
            const string userId = "user1";
            var bookId = Guid.NewGuid();
            var member = new Member { Id = Guid.NewGuid() };
            FakeUser(userId);

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(member);
            _favouriteServiceMock
                .Setup(s => s.AddToFavouritesAsync(member.Id, bookId))
                .ThrowsAsync(new Exception("fail"));

            var result = await _controller.Add(bookId);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            Assert.That(_controller.TempData.ContainsKey("ErrorMessage"), Is.True);
            Assert.That(_controller.TempData["ErrorMessage"], Is.EqualTo(UnexpectedErrorMessage));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(FavouriteListAddErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task Remove_NoUser_ShouldReturnUnauthorized()
        {
            var result = await _controller.Remove(Guid.NewGuid());
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Remove_NoMembership_ShouldReturnUnauthorized()
        {
            const string userId = "user1";
            FakeUser(userId);

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync((Member)null!);

            var result = await _controller.Remove(Guid.NewGuid());
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Remove_WhenRemoved_ShouldRedirectWithSuccessMessage()
        {
            const string userId = "user1";
            var bookId = Guid.NewGuid();
            var member = new Member { Id = Guid.NewGuid() };
            FakeUser(userId);

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(member);

            _favouriteServiceMock
                .Setup(s => s.RemoveFromFavouritesAsync(member.Id, bookId))
                .ReturnsAsync(true);

            var result = await _controller.Remove(bookId);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(FavouriteListController.Index)));

            Assert.That(_controller.TempData.ContainsKey("SuccessMessage"), Is.True);
            Assert.That(_controller.TempData["SuccessMessage"], Is.EqualTo(BookRemovedFromFavourites));
        }

        [Test]
        public async Task Remove_WhenNotInFavourites_ShouldRedirectWithErrorMessage()
        {
            const string userId = "user1";
            var bookId = Guid.NewGuid();
            var member = new Member { Id = Guid.NewGuid() };
            FakeUser(userId);

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(member);

            _favouriteServiceMock
                .Setup(s => s.RemoveFromFavouritesAsync(member.Id, bookId))
                .ReturnsAsync(false);

            var result = await _controller.Remove(bookId);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(FavouriteListController.Index)));

            Assert.That(_controller.TempData.ContainsKey("ErrorMessage"), Is.True);
            Assert.That(_controller.TempData["ErrorMessage"], Is.EqualTo(BookNotFoundInFavourites));
        }

        [Test]
        public async Task Remove_WhenKeyNotFound_ShouldReturnNotFound()
        {
            const string userId = "user1";
            var bookId = Guid.NewGuid();
            var member = new Member { Id = Guid.NewGuid() };
            FakeUser(userId);

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(member);

            _favouriteServiceMock
                .Setup(s => s.RemoveFromFavouritesAsync(member.Id, bookId))
                .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.Remove(bookId);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Remove_WhenExceptionThrown_ShouldLogErrorAndRedirectWithUnexpectedError()
        {
            const string userId = "user1";
            var bookId = Guid.NewGuid();
            var member = new Member { Id = Guid.NewGuid() };
            FakeUser(userId);

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(member);

            _favouriteServiceMock
                .Setup(s => s.RemoveFromFavouritesAsync(member.Id, bookId))
                .ThrowsAsync(new Exception("fail"));

            var result = await _controller.Remove(bookId);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo(nameof(FavouriteListController.Index)));

            Assert.That(_controller.TempData.ContainsKey("ErrorMessage"), Is.True);
            Assert.That(_controller.TempData["ErrorMessage"], Is.EqualTo(UnexpectedErrorMessage));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(FavouriteListRemoveErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }
    }
}
