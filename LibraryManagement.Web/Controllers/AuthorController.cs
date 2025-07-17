using LibraryManagement.Services.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Index(string? searchTerm)
        {
            var authors = await _authorService.GetAuthorsWithBooksAsync(searchTerm);
            return View(authors);
        }
    }
}
