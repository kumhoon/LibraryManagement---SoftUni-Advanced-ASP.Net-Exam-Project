namespace LibraryManagement.Services.Core.Tests.Controllers.ReviewModerationControllerTests
{
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.Areas.Admin.Controllers;
    using static LibraryManagement.GCommon.ErrorMessages;
    using static LibraryManagement.GCommon.Messages.AdminMessages;
    using LibraryManagement.Web.ViewModels.Review;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.Logging;
    using Moq;

    [TestFixture]
    public class ReviewModerationControllerTests
    {
        private Mock<IReviewService> _reviewServiceMock;
        private Mock<ILogger<ReviewModerationController>> _loggerMock;
        private ReviewModerationController _reviewModerationController;

        [SetUp]
        public void SetUp()
        {
            _reviewServiceMock = new Mock<IReviewService>();
            _loggerMock = new Mock<ILogger<ReviewModerationController>>();
            _reviewModerationController = new ReviewModerationController(_reviewServiceMock.Object, _loggerMock.Object);
            _reviewModerationController.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        [TearDown]
        public void TearDown() => _reviewModerationController.Dispose();

        [Test]
        public async Task PendingReviews_WhenServiceSucceeds_ShouldReturnViewWithPendingReviews()
        {
           
            var pendingReviews = new List<PendingReviewViewModel> { new PendingReviewViewModel { ReviewId = Guid.NewGuid(), Rating = 5}};
            _reviewServiceMock.Setup(s => s.GetPendingReviewsAsync()).ReturnsAsync(pendingReviews);

            var result = await _reviewModerationController.PendingReviews();

            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;

            Assert.That(viewResult.Model, Is.AssignableTo<IEnumerable<PendingReviewViewModel>>());
            var model = (IEnumerable<PendingReviewViewModel>)viewResult.Model;

            Assert.That(model.Count(), Is.EqualTo(1));
            Assert.That(model.First().ReviewId, Is.EqualTo(pendingReviews.First().ReviewId));
            Assert.That(model.First().Rating, Is.EqualTo(pendingReviews.First().Rating));
        }

        [Test]
        public async Task PendingReviews_WhenExceptionThrown_ShouldReturnErrorViewAndLogError()
        {
            _reviewServiceMock.Setup(s => s.GetPendingReviewsAsync()).ThrowsAsync(new Exception());

            var result = await _reviewModerationController.PendingReviews();

            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;

            Assert.That(viewResult.ViewName, Is.EqualTo("Error"));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(PendingReviewsLoadingErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task Approve_WhenServiceSucceeds_ShouldRedirectToPendingReviewsWithSuccessMessage()
        {
            var reviewId = Guid.NewGuid();
            _reviewServiceMock.Setup(s => s.ApproveReviewAsync(reviewId)).ReturnsAsync(true);

            var result = await _reviewModerationController.Approve(reviewId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;

            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(_reviewModerationController.PendingReviews)));
            Assert.That(_reviewModerationController.TempData["SuccessMessage"], Is.EqualTo(ReviewApproved));
        }

        [Test]
        public async Task Approve_WhenServiceFails_ShouldRedirectToPendingReviewsWithErrorMessage()
        {
            var reviewId = Guid.NewGuid();
            _reviewServiceMock.Setup(s => s.ApproveReviewAsync(reviewId)).ReturnsAsync(false);

            var result = await _reviewModerationController.Approve(reviewId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;

            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(_reviewModerationController.PendingReviews)));
            Assert.That(_reviewModerationController.TempData["ErrorMessage"], Is.EqualTo(ReviewApproveErrorMessage));

        }
        [Test]
        public async Task Approve_WhenExceptionThrown_ShouldRedirectToPendingReviewsWithErrorMessageAndLogError()
        {
            var reviewId = Guid.NewGuid();
            _reviewServiceMock.Setup(s => s.ApproveReviewAsync(reviewId)).ThrowsAsync(new Exception());

            var result = await _reviewModerationController.Approve(reviewId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;

            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(_reviewModerationController.PendingReviews)));
            Assert.That(_reviewModerationController.TempData["ErrorMessage"], Is.EqualTo(UnexpectedErrorMessage));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(ReviewApproveErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task Reject_WhenServiceSucceeds_ShouldRedirectToPendingReviewsWithSuccessMessage()
        {
            var reviewId = Guid.NewGuid();
            _reviewServiceMock.Setup(s => s.RejectReviewAsync(reviewId)).ReturnsAsync(true);

            var result = await _reviewModerationController.Reject(reviewId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;

            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(_reviewModerationController.PendingReviews)));
            Assert.That(_reviewModerationController.TempData["SuccessMessage"], Is.EqualTo(ReviewRejected));
        }

        [Test]
        public async Task Reject_WhenServiceFails_ShouldRedirectToPendingReviewsWithErrorMessage()
        {
            var reviewId = Guid.NewGuid();
            _reviewServiceMock.Setup(s => s.RejectReviewAsync(reviewId)).ReturnsAsync(false);

            var result = await _reviewModerationController.Reject(reviewId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;

            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(_reviewModerationController.PendingReviews)));
            Assert.That(_reviewModerationController.TempData["ErrorMessage"], Is.EqualTo(ReviewRejectErrorMessage));
        }

        [Test]
        public async Task Reject_WhenExceptionThrown_ShouldRedirectToPendingReviewsWithErrorMessageAndLogError()
        {
            var reviewId = Guid.NewGuid();
            _reviewServiceMock.Setup(s => s.RejectReviewAsync(reviewId)).ThrowsAsync(new Exception());

            var result = await _reviewModerationController.Reject(reviewId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;

            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(_reviewModerationController.PendingReviews)));
            Assert.That(_reviewModerationController.TempData["ErrorMessage"], Is.EqualTo(UnexpectedErrorMessage));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(ReviewRejectErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }
    }
}
