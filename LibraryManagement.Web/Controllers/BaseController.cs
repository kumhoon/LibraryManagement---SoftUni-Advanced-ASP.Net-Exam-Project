namespace LibraryManagement.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    /// <summary>
    /// Serves as a base class for all MVC controllers, providing common authentication utilities.
    /// </summary>
    [Authorize]
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Retrieves the current user's unique identifier from the claims principal.
        /// </summary>
        /// <returns>
        /// The user's <c>ClaimTypes.NameIdentifier</c> value if authenticated; otherwise, <c>null</c>.
        /// </returns>
        protected string? GetUserId()
        {
            string? userId = null;
            bool isAuthenticated = this.IsUserAuthenticated();
            if (isAuthenticated)
            {
                userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            return userId;
        }

        /// <summary>
        /// Determines whether the current user is authenticated.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the user identity is authenticated; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsUserAuthenticated()
        {
            return this.User.Identity?.IsAuthenticated ?? false;
        }
    }
}
