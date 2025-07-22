namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Review;
    using Microsoft.AspNetCore.Mvc;
    using static LibraryManagement.GCommon.Messages.ReviewMessages;
    public class ReviewController : BaseController
    {
        private readonly IReviewService _reviewService;
        private readonly IMembershipService _memberService; 

        public ReviewController(IReviewService reviewService, IMembershipService memberService)
        {
            _reviewService = reviewService;
            _memberService = memberService;
        }

        
        [HttpGet]
        public async Task<IActionResult> Edit(Guid bookId)
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

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReviewInputModel model)
        {
            if (model.Rating < 1 || model.Rating > 5)
            {
                ModelState.AddModelError(nameof(model.Rating), "Rating must be between 1 and 5.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var memberId = await GetCurrentMemberIdAsync();
            if (memberId == Guid.Empty)
                return Unauthorized();

            bool success;

            if (model.ReviewId == Guid.Empty)
            {
               
                success = await _reviewService.CreateReviewAsync(model.BookId, memberId, model.Rating, model.Content);
            }
            else
            {
                
                success = await _reviewService.UpdateReviewAsync(memberId, model.BookId, model.Rating, model.Content);
            }

            if (!success)
            {
                ModelState.AddModelError("", "An error occurred while submitting your review.");
                return View(model);
            }

            TempData["SuccessMessage"] = ReviewAdded;
            return RedirectToAction("Details", "Book", new { id = model.BookId });
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
