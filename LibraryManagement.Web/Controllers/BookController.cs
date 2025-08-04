namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Book;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using static LibraryManagement.GCommon.PagedResultConstants;
    using static LibraryManagement.GCommon.ErrorMessages;

    /// <summary>
    /// Handles book-related operations for listing, viewing details, and CRUD actions.
    /// </summary>
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


        /// <summary>
        /// Displays a paginated list of books, optionally filtered by a search term.
        /// </summary>
        /// <param name="searchTerm">
        /// An optional term to filter books by title or author. If <c>null</c>, all books are shown.
        /// </param>
        /// <param name="pageNumber">The page number to display (default is the first page).</param>
        /// <param name="pageSize">The number of books per page (default is the predefined page size).</param>
        /// <returns>
        /// A view showing the list of books.
        /// Redirects to default pagination if parameters are invalid, or shows an error view on unexpected failures.
        /// </returns>
        /// 
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

        /// <summary>
        /// Displays detailed information for a specific book, including user reviews.
        /// </summary>
        /// <param name="id">The unique identifier of the book.</param>
        /// <param name="pageNumber">The review page number for pagination (default is first page).</param>
        /// <returns>
        /// A view with book details if found;
        /// NotFound if the book does not exist; redirects to the index on unexpected errors.
        /// </returns>
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

        /// <summary>
        /// Shows the form to create a new book (Admin only).
        /// </summary>
        /// <returns>
        /// A view with the create book form populated with available genres;
        /// redirects to index on error.
        /// </returns>
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

        /// <summary>
        /// Processes the submission of a new book (Admin only).
        /// </summary>
        /// <param name="inputModel">The input model containing book data.</param>
        /// <returns>
        /// Redirects to index on success;
        /// re-displays the form with validation messages on failure.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        /// <summary>
        /// Shows the form to edit an existing book (Admin only).
        /// </summary>
        /// <param name="id">The unique identifier of the book to edit.</param>
        /// <returns>
        /// A view with the edit form populated with current book data;
        /// NotFound, Forbid, or error view on failures.
        /// </returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

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

        /// <summary>
        /// Processes the submission of edited book data (Admin only).
        /// </summary>
        /// <param name="editInputModel">The input model with updated book data.</param>
        /// <returns>
        /// Redirects to the details view on success;
        /// re-displays the form with error messages on validation or update failures.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        /// <summary>
        /// Displays a confirmation view for deleting a book (Admin only).
        /// </summary>
        /// <param name="id">The unique identifier of the book to delete.</param>
        /// <returns>
        /// A view with delete confirmation data;
        /// NotFound, Forbid, or error view on failures.
        /// </returns>
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

        /// <summary>
        /// Processes the deletion request for a book (Admin only).
        /// </summary>
        /// <param name="inputModel">The input model containing delete confirmation data.</param>
        /// <returns>
        /// Redirects to index on success;
        /// re-displays the delete confirmation view with errors on failure.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
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
