namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.User;
    using Microsoft.AspNetCore.Mvc;
    

    public class UserController : BaseController
    {
        private readonly IMembershipService _membershipService;
        public UserController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }
        public async Task<IActionResult> Dashboard()
        {
            var userId = this.GetUserId()!;
            var membership = await _membershipService.GetMembershipByUserIdAsync(userId);

            var viewModel = new UserMembershipViewModel
            {
                MembershipStatus = membership?.Status
            };

            return View(viewModel);
        }
    }
}
