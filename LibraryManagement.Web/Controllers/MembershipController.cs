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
                var userId = this.GetUserId()!;
                var membership = _membershipService.GetMembershipByUserIdAsync(userId);

                ViewBag.CanApply = membership == null; 

                var model = new MemberApplicationInputModel
                {
                    Name = this.User.Identity!.Name!.Split('@')[0], 
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
