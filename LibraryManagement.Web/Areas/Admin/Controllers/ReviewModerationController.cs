namespace LibraryManagement.Web.Areas.Admin.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.Controllers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using static LibraryManagement.GCommon.ErrorMessages;
    using static LibraryManagement.GCommon.Messages.AdminMessages;
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReviewModerationController : BaseController
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewModerationController> _logger;

        public ReviewModerationController(IReviewService reviewService, ILogger<ReviewModerationController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> PendingReviews()
        {
            try
            {
                var pending = await _reviewService.GetPendingReviewsAsync();
                return View(pending);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, PendingReviewsLoadingErrorMessage);          
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Approve(Guid id)
        {
            try
            {
                bool success = await _reviewService.ApproveReviewAsync(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = ReviewApproveErrorMessage;
                    return RedirectToAction(nameof(PendingReviews));
                }
                TempData["SuccessMessage"] = ReviewApproved;
                return RedirectToAction(nameof(PendingReviews));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ReviewApproveErrorMessage);
                TempData["ErrorMessage"] = UnexpectedErrorMessage;
                return RedirectToAction(nameof(PendingReviews));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Reject(Guid id)
        {
            try
            {
                bool success = await _reviewService.RejectReviewAsync(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = ReviewRejectErrorMessage;
                    return RedirectToAction(nameof(PendingReviews));
                }
                TempData["SuccessMessage"] = ReviewRejected;
                return RedirectToAction(nameof(PendingReviews));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ReviewRejectErrorMessage);
                TempData["ErrorMessage"] = UnexpectedErrorMessage;
                return RedirectToAction(nameof(PendingReviews));
            }
        }
    }
}
