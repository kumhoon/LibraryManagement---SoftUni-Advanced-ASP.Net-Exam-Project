namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class FavouriteListController : BaseController
    {
        private readonly IFavouriteListService _favouriteListService;
        private readonly IMembershipService _membershipService;

        public FavouriteListController(IFavouriteListService favouriteListService, IMembershipService membershipService)
        {
            _favouriteListService = favouriteListService;
            _membershipService = membershipService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string? userId = this.GetUserId();
            if (string.IsNullOrEmpty(userId))
            { 
                return Unauthorized(); 
            }

            var member = await _membershipService.GetMembershipByUserIdAsync(userId);

            if (member == null)
            {               
                return RedirectToAction("Index", "Book");
            }

            var favouriteBooks = await _favouriteListService.GetFavouriteBooksAsync(member.Id);

            return View(favouriteBooks);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Guid bookId)
        {
            string? userId = this.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var member = await _membershipService.GetMembershipByUserIdAsync(userId);

            if (member == null)
            {
                return Unauthorized();
            }

            bool added = await _favouriteListService.AddToFavouritesAsync(member.Id, bookId);

            TempData[added ? "SuccessMessage" : "ErrorMessage"] =
            added ? "Book added to your favourites!" : "This book is already in your favourites.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Remove(Guid bookId)
        {
            string? userId = this.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var member = await _membershipService.GetMembershipByUserIdAsync(userId);

            if (member == null)
            {
                return Unauthorized();
            }

            bool removed = await _favouriteListService.RemoveFromFavouritesAsync(member.Id, bookId);

            TempData[removed ? "SuccessMessage" : "ErrorMessage"] =
            removed ? "Book removed from your favourites." : "This book was not found in your favourites.";

            return RedirectToAction(nameof(Index));
        }
    }
}
