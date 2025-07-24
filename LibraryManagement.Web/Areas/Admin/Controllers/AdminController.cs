namespace LibraryManagement.Web.Areas.Admin.Controllers
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Core.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using LibraryManagement.Web.Controllers;
    using static LibraryManagement.GCommon.Messages.AdminMessages;
    using static LibraryManagement.GCommon.ErrorMessages;

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

        [HttpPost]
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

        [HttpPost]
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

        [HttpPost]
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
