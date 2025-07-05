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
        private readonly IGenreService _genreService;

        public BookController(IBookService bookService, IGenreService genreService)
        {
            _bookService = bookService;
            _genreService = genreService;
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
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                string? userId = this.GetUserId();
                BookDetailsViewModel bookDetailsVM = await this._bookService.GetBookDetailsAsync(id, userId);
                if (bookDetailsVM == null)
                {         
                    return NotFound();
                }
                return View(bookDetailsVM);
            }
            catch (Exception e)
            {

                Console.WriteLine($"An error has occured: {e.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                BookCreateInputModel? inputModel = new BookCreateInputModel
                {
                    Genres = await _genreService.GetAllAsSelectListAsync()
                };
                return View(inputModel);
            }
            catch (Exception e)
            {

                Console.WriteLine($"An error has occured: {e.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
