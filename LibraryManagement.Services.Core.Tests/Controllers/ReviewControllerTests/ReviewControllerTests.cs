namespace LibraryManagement.Services.Core.Tests.Controllers.ReviewControllerTests
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.Controllers;
    using LibraryManagement.Web.ViewModels.Review;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System.Security.Claims;
    using static LibraryManagement.GCommon.ErrorMessages;

    [TestFixture]
    public class ReviewControllerTests
    {
        private Mock<IReviewService> _reviewServiceMock;
        private Mock<IMembershipService> _membershipServiceMock;
        private Mock<ILogger<ReviewController>> _loggerMock;
        private ReviewController _controller;

        [SetUp]
        public void SetUp()
        {
            _reviewServiceMock = new Mock<IReviewService>();
            _membershipServiceMock = new Mock<IMembershipService>();
            _loggerMock = new Mock<ILogger<ReviewController>>();

            _controller = new ReviewController(
                _reviewServiceMock.Object,
                _membershipServiceMock.Object,
                _loggerMock.Object
            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            _controller.TempData = new TempDataDictionary(_controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
        }

        [TearDown]
        public void TearDown() => _controller.Dispose();

        private void FakeUser(string userId)
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));
            _controller.ControllerContext.HttpContext.User = principal;
        }

        [Test]
        public async Task Edit_WhenBookIdIsEmpty_ShouldReturnBadRequest()
        {
            var result = await _controller.Edit(Guid.Empty);

            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task Edit_WhenNoMemberFound_ShouldReturnUnauthorized()
        {

            FakeUser("user123");
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync("user123"))
                .ReturnsAsync((Member)null!);

            var result = await _controller.Edit(Guid.NewGuid());

            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Edit_WhenReviewExists_ShouldReturnViewWithModel()
        {

            var userId = "user123";
            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            FakeUser(userId);
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(new Member { Id = memberId });

            _reviewServiceMock
                .Setup(s => s.GetMemberReviewForBookAsync(memberId, bookId))
                .ReturnsAsync(new ReviewInputModel
                {
                    ReviewId = Guid.NewGuid(),
                    Rating = 4,
                    Content = "Good book"
                });

            var result = await _controller.Edit(bookId);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var view = (ViewResult)result;
            var model = (ReviewInputModel)view.Model;

            Assert.That(model.BookId, Is.EqualTo(bookId));
            Assert.That(model.Rating, Is.EqualTo(4));
            Assert.That(model.Content, Is.EqualTo("Good book"));
        }

        [Test]
        public async Task Edit_WhenNoReviewExists_ShouldReturnDefaultReviewModel()
        {

            var userId = "user123";
            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            FakeUser(userId);
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(new Member { Id = memberId });

            _reviewServiceMock
                .Setup(s => s.GetMemberReviewForBookAsync(memberId, bookId))
                .ReturnsAsync((ReviewInputModel)null!);

            var result = await _controller.Edit(bookId);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var view = (ViewResult)result;
            var model = (ReviewInputModel)view.Model;

            Assert.That(model.BookId, Is.EqualTo(bookId));
            Assert.That(model.Rating, Is.EqualTo(5)); 
            Assert.That(model.ReviewId, Is.EqualTo(Guid.Empty));
        }

        [Test]
        public async Task Edit_WhenExceptionThrown_ShouldLogErrorAndReturnErrorView()
        {

            var userId = "user123";
            FakeUser(userId);

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ThrowsAsync(new Exception("DB error"));

            var result = await _controller.Edit(Guid.NewGuid());

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var view = (ViewResult)result;
            Assert.That(view.ViewName, Is.EqualTo("Error"));

            _loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(UnexpectedErrorMessage)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Test]
        public async Task EditPost_WhenRatingInvalid_ShouldReturnViewWithModelError()
        {

            var model = new ReviewInputModel { Rating = 6, BookId = Guid.NewGuid() };

            var result = await _controller.Edit(model);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            Assert.That(!_controller.ModelState.IsValid, Is.True);
            Assert.That(_controller.ModelState.ContainsKey(nameof(model.Rating)), Is.True);
        }

        [Test]
        public async Task EditPost_WhenModelStateInvalid_ShouldReturnView()
        {

            var model = new ReviewInputModel { Rating = 3 };
            _controller.ModelState.AddModelError("Content", "Required");

            var result = await _controller.Edit(model);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var view = (ViewResult)result;
            Assert.That(view.Model, Is.EqualTo(model));
        }

        [Test]
        public async Task EditPost_WhenMemberNotFound_ShouldReturnUnauthorized()
        {

            var model = new ReviewInputModel { Rating = 3, BookId = Guid.NewGuid() };
            FakeUser("user123");
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync("user123"))
                .ReturnsAsync((Member)null!);

            var result = await _controller.Edit(model);

            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task EditPost_WhenCreatingReviewSucceeds_ShouldRedirectToBookDetails()
        {

            var userId = "user123";
            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var model = new ReviewInputModel { Rating = 4, BookId = bookId };

            FakeUser(userId);
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(new Member { Id = memberId });
            _reviewServiceMock
                .Setup(s => s.CreateReviewAsync(bookId, memberId, 4, null))
                .ReturnsAsync(true);

            var result = await _controller.Edit(model);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo("Details"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Book"));
            Assert.That(redirect.RouteValues["id"], Is.EqualTo(bookId));
        }

        [Test]
        public async Task EditPost_WhenUpdatingReviewSucceeds_ShouldRedirectToBookDetails()
        {

            var userId = "user123";
            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var model = new ReviewInputModel
            {
                ReviewId = Guid.NewGuid(),
                Rating = 4,
                BookId = bookId,
                Content = "Updated review" 
            };

            FakeUser(userId);
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(new Member { Id = memberId });

            _reviewServiceMock
                .Setup(s => s.UpdateReviewAsync(memberId, bookId, 4, "Updated review"))
                .ReturnsAsync(true);


            _controller.ModelState.Clear(); 
            var result = await _controller.Edit(model);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo("Details"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Book"));
        }

        [Test]
        public async Task EditPost_WhenServiceFails_ShouldReturnViewWithError()
        {

            var userId = "user123";
            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var model = new ReviewInputModel { Rating = 4, BookId = bookId };

            FakeUser(userId);
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(new Member { Id = memberId });
            _reviewServiceMock
                .Setup(s => s.CreateReviewAsync(bookId, memberId, 4, null))
                .ReturnsAsync(false);

            var result = await _controller.Edit(model);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            Assert.That(_controller.ModelState.ContainsKey(string.Empty), Is.True);
        }

        [Test]
        public async Task EditPost_WhenExceptionThrown_ShouldLogErrorAndReturnView()
        {

            var userId = "user123";
            FakeUser(userId);
            var model = new ReviewInputModel { Rating = 4, BookId = Guid.NewGuid() };

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ThrowsAsync(new Exception("DB error"));

            var result = await _controller.Edit(model);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            Assert.That(_controller.ModelState.ContainsKey(string.Empty), Is.True);

            _loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(ReviewSubmitErrorMessage)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }
    }
}
