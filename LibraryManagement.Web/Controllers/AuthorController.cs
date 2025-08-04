namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using static LibraryManagement.GCommon.PagedResultConstants;
    using static LibraryManagement.GCommon.ErrorMessages;

    /// <summary>
    /// Provides read-only operations for displaying authors and their associated books.
    /// </summary>
    public class AuthorController : BaseController
    {
        private readonly IAuthorService _authorService;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(IAuthorService authorService, ILogger<AuthorController> logger)
        {
            _authorService = authorService;
            _logger = logger;
        }


        /// <summary>
        /// Displays a paginated list of authors along with their books, optionally filtered by a search term.
        /// </summary>
        /// <param name="searchTerm">
        /// An optional term to filter authors by name.  
        /// If <c>null</c> or empty, all authors are shown.
        /// </param>
        /// <param name="pageNumber">The page number to display (default is the first page).</param>
        /// <param name="pageSize">The number of authors per page (default is the predefined page size).</param>
        /// <returns>
        /// A view displaying the authors and their books.  
        /// Redirects to a default page if pagination parameters are invalid,  
        /// or shows an error view on unexpected failures.
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? searchTerm, int pageNumber = DefaultPageNumber, int pageSize = DefaultPageSize)
        {
            try
            {
                var authors = await _authorService.GetAuthorsWithBooksAsync(searchTerm, pageNumber, pageSize);
                ViewData["SearchTerm"] = searchTerm;
                return View(authors);
            }
            catch (ArgumentOutOfRangeException)
            {
                
                TempData["ErrorMessage"] = InvalidPaginationValues;
                return RedirectToAction("Index", new { pageNumber = DefaultPageNumber, pageSize = DefaultPageSize });
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, LoadingAuthorsErrorMessage);
                return View("Error"); 
            }
        }
    }
}
