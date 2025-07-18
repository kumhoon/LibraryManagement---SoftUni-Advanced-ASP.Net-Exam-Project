namespace LibraryManagement.Web.Areas.Admin.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.Controllers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReviewModerationController : BaseController
    {
        private readonly IReviewService _reviewService;

        public ReviewModerationController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [HttpGet]
        public async Task<IActionResult> PendingReviews()
        {
            var pending = await _reviewService.GetPendingReviewsAsync();
            return View(pending);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _reviewService.ApproveReviewAsync(id);
            return RedirectToAction(nameof(PendingReviews));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(Guid id)
        {
            await _reviewService.RejectReviewAsync(id);
            return RedirectToAction(nameof(PendingReviews));
        }
    }
}
