namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Book;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using static LibraryManagement.GCommon.PagedResultConstants;
    using static LibraryManagement.GCommon.ErrorMessages;
    public class BookController : BaseController
    {
        private readonly IBookService _bookService;
        private readonly IGenreService _genreService;
        private readonly ILogger<BookController> _logger;

        public BookController(IBookService bookService, IGenreService genreService, ILogger<BookController> logger)
        {
            _bookService = bookService;
            _genreService = genreService;
            _logger = logger;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? searchTerm, int pageNumber = DefaultPageNumber, int pageSize = DefaultPageSize)
        {
            try
            {
                var result = await _bookService.GetBookIndexAsync(searchTerm, pageNumber, pageSize);
                ViewData["SearchTerm"] = searchTerm;
                return View(result);
            }
            catch (ArgumentOutOfRangeException ex)
            {                
                _logger.LogWarning(ex, InvalidPaginationValues);
                return RedirectToAction(nameof(Index), new { searchTerm, pageNumber = DefaultPageNumber, pageSize = DefaultPageSize });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, UnexpectedErrorMessage);
                return View("Error");
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id, int pageNumber = DefaultPageNumber)
        {
            try
            {
                string? userId = this.GetUserId();
                BookDetailsViewModel? bookDetailsVM = await this._bookService.GetBookDetailsAsync(id, userId, pageNumber);
                if (bookDetailsVM == null)
                {         
                    return NotFound();
                }
                return View(bookDetailsVM);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, UnexpectedErrorMessage);
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            try
            {
                BookCreateInputModel? inputModel = new()
                {
                    Genres = await _genreService.GetAllAsSelectListAsync()
                };
                return View(inputModel);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, UnexpectedErrorMessage);
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(BookCreateInputModel inputModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    inputModel.Genres = await this._genreService.GetAllAsSelectListAsync();
                    return View(inputModel);
                }
                var (success, reason) = await _bookService.CreateBookAsync(GetUserId()!, inputModel);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, reason ?? UnexpectedErrorMessage);
                    inputModel.Genres = await _genreService.GetAllAsSelectListAsync();
                    return View(inputModel);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, UnexpectedErrorMessage);
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            try
            {
                var userId = GetUserId()!;
                var editInputModel = await _bookService.GetBookForEditingAsync(userId, id.Value);
                editInputModel.Genres = await _genreService.GetAllAsSelectListAsync();

                return View(editInputModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, BookEditErrorMessage);
                return View("Error");
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(BookEditInputModel editInputModel)
        {
            if (!ModelState.IsValid)
            {
                editInputModel.Genres = await _genreService.GetAllAsSelectListAsync();
                return View(editInputModel);
            }

            try
            {
                await _bookService.UpdateEditedBookAsync(GetUserId()!, editInputModel);
                return RedirectToAction(nameof(Details), new { id = editInputModel.Id });
            }
            catch (FormatException)
            {
                ModelState.AddModelError(nameof(editInputModel.PublishedDate), InvalidPublishedDateFormatErrorMessage);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, BookUpdateErrorMessage);
                ModelState.AddModelError(string.Empty, UnexpectedErrorMessage);
            }

            editInputModel.Genres = await _genreService.GetAllAsSelectListAsync();
            return View(editInputModel);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                string userId = GetUserId()!;
                var bookDeleteInputModel = await _bookService.GetBookForDeletingAsync(userId, id);
                return View(bookDeleteInputModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, UnexpectedErrorMessage);
                return View("Error");
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmDelete(BookDeleteInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, InvalidDataErrorMessage);
                return View(inputModel);
            }

            try
            {
                await _bookService.SoftDeleteBookAsync(GetUserId()!, inputModel);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, BookDeleteErrorMessage);
                ModelState.AddModelError(string.Empty, UnexpectedErrorMessage);
                return View(inputModel);
            }
        }
    }
}
