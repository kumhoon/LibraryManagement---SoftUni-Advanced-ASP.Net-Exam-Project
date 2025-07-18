using LibraryManagement.Services.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagement.Web.Controllers
{
    public class AuthorController : BaseController
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? searchTerm, int page = 1, int pageSize = 5)
        {
            var authors = await _authorService.GetAuthorsWithBooksAsync(searchTerm, page, pageSize);
            ViewData["SearchTerm"] = searchTerm;
            return View(authors);
        }
    }
}
