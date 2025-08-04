namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.User;
    using Microsoft.AspNetCore.Mvc;
    using static LibraryManagement.GCommon.ErrorMessages;

    /// <summary>
    /// Provides users a personal dashboard and view their membership status.
    /// </summary>
    public class UserController : BaseController
    {
        private readonly IMembershipService _membershipService;
        private readonly ILogger<UserController> _logger;
        public UserController(IMembershipService membershipService, ILogger<UserController> logger)
        {
            _membershipService = membershipService;
            _logger = logger;
        }

        /// <summary>
        /// Displays the user's dashboard with their membership status.
        /// </summary>
        /// <returns>
        /// A view containing a <see cref="UserMembershipViewModel"/> with membership status; 
        /// returns an error view if an unexpected exception occurs.
        /// </returns>
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var userId = this.GetUserId()!;
                var membership = await _membershipService.GetMembershipByUserIdAsync(userId);

                var viewModel = new UserMembershipViewModel
                {
                    MembershipStatus = membership?.Status
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, UserDashboardErrorMessage);
                TempData["ErrorMessage"] = UnexpectedErrorMessage;
                return View("Error"); 
            }
        }
    }
}
