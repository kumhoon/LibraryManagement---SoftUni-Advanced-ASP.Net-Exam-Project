namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using static LibraryManagement.GCommon.Messages.FavouriteListMessages;
    using static LibraryManagement.GCommon.ErrorMessages;
    public class FavouriteListController : BaseController
    {
        private readonly IFavouriteListService _favouriteListService;
        private readonly IMembershipService _membershipService;
        private readonly ILogger<FavouriteListController> _logger;

        public FavouriteListController(IFavouriteListService favouriteListService, IMembershipService membershipService, ILogger<FavouriteListController> logger)
        {
            _favouriteListService = favouriteListService;
            _membershipService = membershipService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = this.GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var member = await _membershipService.GetMembershipByUserIdAsync(userId);
                if (member == null)
                    return RedirectToAction("Index", "Book");

                var favouriteBooks = await _favouriteListService.GetFavouriteBooksAsync(member.Id);
                return View(favouriteBooks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, FavouriteListErrorMessage);
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(Guid bookId)
        {
            try
            {
                var userId = this.GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var member = await _membershipService.GetMembershipByUserIdAsync(userId);
                if (member == null)
                    return Unauthorized();

                bool added = await _favouriteListService.AddToFavouritesAsync(member.Id, bookId);

                TempData[added ? "SuccessMessage" : "ErrorMessage"] =
                    added ? BookAddedToFavourites : BookAlreadyInFavourites;

                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, FavouriteListAddErrorMessage);
                TempData["ErrorMessage"] = UnexpectedErrorMessage;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Remove(Guid bookId)
        {
            try
            {
                var userId = this.GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var member = await _membershipService.GetMembershipByUserIdAsync(userId);
                if (member == null)
                    return Unauthorized();

                bool removed = await _favouriteListService.RemoveFromFavouritesAsync(member.Id, bookId);

                TempData[removed ? "SuccessMessage" : "ErrorMessage"] =
                    removed ? BookRemovedFromFavourites : BookNotFoundInFavourites;

                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, FavouriteListRemoveErrorMessage);
                TempData["ErrorMessage"] = UnexpectedErrorMessage;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
