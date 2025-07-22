namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using static LibraryManagement.GCommon.PagedResultConstants;
    public class AuthorController : BaseController
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? searchTerm, int pageNumber = DefaultPageNumber, int pageSize = DefaultPageSize)
        {
            var authors = await _authorService.GetAuthorsWithBooksAsync(searchTerm, pageNumber, pageSize);
            ViewData["SearchTerm"] = searchTerm;
            return View(authors);
        }
    }
}
