namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Membership;
    using Microsoft.AspNetCore.Mvc;
    using static LibraryManagement.GCommon.ErrorMessages;
    public class MembershipController : BaseController
    {
        private readonly IMembershipService _membershipService;
        private readonly ILogger<MembershipController> _logger;

        public MembershipController(IMembershipService membershipService, ILogger<MembershipController> logger)
        {
            _membershipService = membershipService;
            _logger = logger;
        }

        [HttpGet]

        public async Task<IActionResult> Apply()
        {
            try
            {
                var userId = this.GetUserId()!;
                var membership = await _membershipService.GetMembershipByUserIdAsync(userId);

                bool canApply = membership == null ||
                                membership.Status == MembershipStatus.Rejected ||
                                membership.Status == MembershipStatus.Revoked;

                ViewBag.CanApply = canApply;

                var model = new MemberApplicationInputModel
                {
                    Name = this.User.Identity!.Name!.Split('@')[0],
                };

                return View(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e, MembershipApplicationPageErrorMessage);
                TempData["ErrorMessage"] = UnexpectedErrorMessage;
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(MemberApplicationInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return View(inputModel);
            }

            try
            {
                var userId = this.GetUserId()!;
                await _membershipService.ApplyForMembershipAsync(userId, inputModel);
                return RedirectToAction("ApplyConfirmation");
            }
            catch (InvalidOperationException ex)
            {     
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(inputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MembershipApplicationErrorMessage);
                TempData["ErrorMessage"] = UnexpectedErrorMessage;
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult ApplyConfirmation()
        {
            return View();
        }
    }
}
