namespace LibraryManagement.Web.Areas.Admin.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.Controllers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using static LibraryManagement.GCommon.ErrorMessages;
    using static LibraryManagement.GCommon.Messages.AdminMessages;

    /// <summary>
    /// Provides administrative actions for moderating pending book reviews.
    /// </summary>
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

        /// <summary>
        /// Displays a list of reviews that are pending moderation.
        /// </summary>
        /// <returns>
        /// A view showing all pending reviews.  
        /// Returns the "Error" view if loading fails.
        /// </returns>
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

        /// <summary>
        /// Approves a specific pending review, making it visible to users.
        /// </summary>
        /// <param name="id">The unique identifier of the review to approve.</param>
        /// <returns>
        /// Redirects to the PendingReviews view.  
        /// Displays a success or error message based on the result.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        /// <summary>
        /// Rejects a specific pending review, preventing it from being displayed to users.
        /// </summary>
        /// <param name="id">The unique identifier of the review to reject.</param>
        /// <returns>
        /// Redirects to the PendingReviews view.  
        /// Displays a success or error message based on the result.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
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
