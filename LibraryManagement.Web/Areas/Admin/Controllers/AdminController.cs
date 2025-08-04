namespace LibraryManagement.Web.Areas.Admin.Controllers
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Core.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using LibraryManagement.Web.Controllers;
    using static LibraryManagement.GCommon.Messages.AdminMessages;
    using static LibraryManagement.GCommon.ErrorMessages;

    /// <summary>
    /// Provides administrative functionalities for managing memberships.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IMembershipService _membershipService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IMembershipService membershipService, ILogger<AdminController> logger)
        {
            _membershipService = membershipService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View(); 
        }

        /// <summary>
        /// Displays a list of approved members.
        /// </summary>
        /// <returns>
        /// A view showing all approved members.  
        /// Redirects to the dashboard if an unexpected error occurs.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Members()
        {
            try
            {
                var members = await _membershipService.GetApprovedMembersAsync();
                return View(members);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, UnexpectedErrorMessage);
                TempData["ErrorMessage"] = UnexpectedErrorMessage;
                return RedirectToAction(nameof(Dashboard));
            }
            
        }

        /// <summary>
        /// Displays a list of pending membership applications for review.
        /// </summary>
        /// <returns>
        /// A view showing all pending membership applications.  
        /// Redirects to the dashboard if an unexpected error occurs.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> ReviewApplications()
        {
            try
            {
                var pendingMembers = await _membershipService.GetPendingApplications();
                return View(pendingMembers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, UnexpectedErrorMessage);
                TempData["ErrorMessage"] = UnexpectedErrorMessage;
                return RedirectToAction(nameof(Dashboard));
            }
            
        }

        /// <summary>
        /// Approves a pending membership application.
        /// </summary>
        /// <param name="id">The unique identifier of the member application to approve.</param>
        /// <returns>
        /// Redirects to the ReviewApplications view.  
        /// Displays a success or error message based on the operation result.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveMembership(Guid id)
        {
            try
            {
                bool success = await _membershipService.UpdateMembershipStatusAsync(id, MembershipStatus.Approved);

                TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                    success ? MembershipApproved : MembershipApprovedFailed;

                return RedirectToAction(nameof(ReviewApplications));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApproveMembershipErrorMessage);
                TempData["ErrorMessage"] = UnexpectedErrorMessage;
                return RedirectToAction(nameof(ReviewApplications));
            }
        }

        /// <summary>
        /// Rejects a pending membership application.
        /// </summary>
        /// <param name="id">The unique identifier of the member application to reject.</param>
        /// <returns>
        /// Redirects to the ReviewApplications view.  
        /// Displays a success or error message based on the operation result.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectMembership(Guid id)
        {
            try
            {
                bool success = await _membershipService.UpdateMembershipStatusAsync(id, MembershipStatus.Rejected);

                TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                    success ? MembershipRejected : MembershipRejectedFailed;

                return RedirectToAction(nameof(ReviewApplications));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, RejectMembershipErrorMessage);
                TempData["ErrorMessage"] = UnexpectedErrorMessage;
                return RedirectToAction(nameof(ReviewApplications));
            }
        }

        /// <summary>
        /// Revokes the membership of an existing approved member.
        /// </summary>
        /// <param name="id">The unique identifier of the member whose membership will be revoked.</param>
        /// <returns>
        /// Redirects to the Members view.  
        /// Displays a success or error message based on the operation result.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeMembership(Guid id)
        {
            try
            {
                bool success = await _membershipService.UpdateMembershipStatusAsync(id, MembershipStatus.Revoked);

                TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                    success ? MembershipRevoked : MembershipRevokedFailed;

                return RedirectToAction(nameof(Members));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, RevokeMembershipErrorMessage);
                TempData["ErrorMessage"] = UnexpectedErrorMessage;
                return RedirectToAction(nameof(Members));
            }
        }
    }
}
