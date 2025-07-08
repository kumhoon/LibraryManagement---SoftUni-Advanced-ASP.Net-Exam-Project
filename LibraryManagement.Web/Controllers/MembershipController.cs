namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Membership;
    using Microsoft.AspNetCore.Mvc;
    public class MembershipController : BaseController
    {
        private readonly IMembershipService _membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        [HttpGet]
        public IActionResult Apply()
        {
            try
            {
                var email = User.Identity?.Name ?? "";
                var defaultName = email.Contains('@') ? email.Split('@')[0] : email;

                var model = new MemberApplicationInputModel
                {
                    Name = defaultName,
                };

                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error has occured: {e.Message}");
                return RedirectToAction(nameof(Apply));
            }
        }
        [HttpPost]
        public async Task<IActionResult> Apply(MemberApplicationInputModel inputmodel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(inputmodel);
                }
                var userId = this.GetUserId()!;

                await _membershipService.ApplyForMembershipAsync(userId, inputmodel);
                TempData["SuccessMessage"] = "Your membership application has been submitted.";
                return RedirectToAction("ApplyConfirmation");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error has occured: {e.Message}");
                return RedirectToAction(nameof(Apply));
            }
        }
        public IActionResult ApplyConfirmation()
        {
            return View();
        }
    }
}
