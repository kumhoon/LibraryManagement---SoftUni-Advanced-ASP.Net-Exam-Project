namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Core.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IMembershipService _membershipService;

        public AdminController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View(); 
        }

        [HttpGet]
        public async Task<IActionResult> Members()
        {
            var members = await this._membershipService.GetApprovedMembersAsync();
            return View(members);
        }
        [HttpGet]
        public async Task<IActionResult> ReviewApplications()
        {
            var pendingMembers = await this._membershipService.GetPendingApplications();
            return View(pendingMembers);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveMembership(Guid id)
        {
            await this._membershipService.UpdateMembershipStatusAsync(id, MembershipStatus.Approved);
            return RedirectToAction(nameof(ReviewApplications));
        }
        [HttpPost]
        public async Task<IActionResult> RejectMembership(Guid id)
        {
            await this._membershipService.UpdateMembershipStatusAsync(id, MembershipStatus.Rejected);
            return RedirectToAction(nameof(ReviewApplications));
        }
        [HttpPost]
        public async Task<IActionResult> RevokeMembership(Guid id)
        {
            await this._membershipService.UpdateMembershipStatusAsync(id, MembershipStatus.Revoked);
            return RedirectToAction(nameof(Members));
        }
    }
}
