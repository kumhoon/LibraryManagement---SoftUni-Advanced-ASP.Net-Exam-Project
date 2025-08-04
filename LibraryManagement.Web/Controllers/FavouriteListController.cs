namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using static LibraryManagement.GCommon.Messages.FavouriteListMessages;
    using static LibraryManagement.GCommon.ErrorMessages;

    /// <summary>
    /// Manages the favourite books list for authenticated members, including viewing, adding, and removing favourites.
    /// </summary>
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

        /// <summary>
        /// Displays the list of favourite books for the current member.
        /// </summary>
        /// <returns>
        /// A view showing the member's favourite books;  
        /// returns <c>Unauthorized</c> if not logged in; redirects to the book index if no membership exists.
        /// </returns>
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

        /// <summary>
        /// Adds a book to the current member's favourites list.
        /// </summary>
        /// <param name="bookId">The unique identifier of the book to add.</param>
        /// <returns>
        /// Redirects to <c>Index</c> with a success or error message;  
        /// returns <c>Unauthorized</c> if not logged in or not a member; returns <c>NotFound</c> if book not found.
        /// </returns>
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

        /// <summary>
        /// Removes a book from the current member's favourites list.
        /// </summary>
        /// <param name="bookId">The unique identifier of the book to remove.</param>
        /// <returns>
        /// Redirects to <c>Index</c> with a success or error message;  
        /// returns <c>Unauthorized</c> if not logged in or not a member; returns <c>NotFound</c> if book not in favourites.
        /// </returns>
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
