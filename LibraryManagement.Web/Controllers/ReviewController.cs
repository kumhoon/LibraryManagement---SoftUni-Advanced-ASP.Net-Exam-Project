namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Review;
    using Microsoft.AspNetCore.Mvc;
    using static LibraryManagement.GCommon.Messages.ReviewMessages;
    using static LibraryManagement.GCommon.ErrorMessages;
    public class ReviewController : BaseController
    {
        private readonly IReviewService _reviewService;
        private readonly IMembershipService _memberService; 
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IReviewService reviewService, IMembershipService memberService, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _memberService = memberService;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid bookId)
        {
            if (bookId == Guid.Empty)
                return BadRequest();

            try
            {
                var memberId = await GetCurrentMemberIdAsync();
                if (memberId == Guid.Empty)
                    return Unauthorized();

                var review = await _reviewService.GetMemberReviewForBookAsync(memberId, bookId);

                var model = new ReviewInputModel
                {
                    ReviewId = review?.ReviewId ?? Guid.Empty,
                    Rating = review?.Rating ?? 5,
                    Content = review?.Content,
                    BookId = bookId
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, UnexpectedErrorMessage);
                return View("Error");  
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReviewInputModel model)
        {
            try
            {
                if (model.Rating < 1 || model.Rating > 5)
                {
                    ModelState.AddModelError(nameof(model.Rating), ReviewRatingErrorMessage);
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var memberId = await GetCurrentMemberIdAsync();
                if (memberId == Guid.Empty)
                    return Unauthorized();

                bool success = model.ReviewId == Guid.Empty
                    ? await _reviewService.CreateReviewAsync(model.BookId, memberId, model.Rating, model.Content)
                    : await _reviewService.UpdateReviewAsync(memberId, model.BookId, model.Rating, model.Content);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, ReviewSubmitErrorMessage);
                    return View(model);
                }

                ModelState.Clear();

                TempData["SuccessMessage"] = ReviewAdded;
                return RedirectToAction("Details", "Book", new { id = model.BookId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ReviewSubmitErrorMessage);
                ModelState.AddModelError(string.Empty, UnexpectedErrorMessage);
                return View(model);
            }
        }


        private async Task<Guid> GetCurrentMemberIdAsync()
        {
            string? userId = this.GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return Guid.Empty;

            var member = await _memberService.GetMembershipByUserIdAsync(userId);
            return member?.Id ?? Guid.Empty;
        }
    }
}
