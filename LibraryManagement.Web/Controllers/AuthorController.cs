namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using static LibraryManagement.GCommon.PagedResultConstants;
    using static LibraryManagement.GCommon.ErrorMessages;
    public class AuthorController : BaseController
    {
        private readonly IAuthorService _authorService;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(IAuthorService authorService, ILogger<AuthorController> logger)
        {
            _authorService = authorService;
            _logger = logger;
        }

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
