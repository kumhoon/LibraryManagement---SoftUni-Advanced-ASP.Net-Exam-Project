using LibraryManagement.Services.Core.Interfaces;
using LibraryManagement.Web.ViewModels.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibraryManagement.Web.Controllers
{
    public class BookController : BaseController
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            try
            {
                string? userId = this.GetUserId();
                IEnumerable<BookIndexViewModel> allBooks = await this._bookService.GetBookIndexAsync(userId);
                return View(allBooks);
            }
            catch (Exception e)
            {

                Console.WriteLine($"An error has occured: {e.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
