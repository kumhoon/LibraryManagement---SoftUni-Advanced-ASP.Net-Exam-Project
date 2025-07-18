﻿namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Book;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Index(string? searchTerm, int page = 1, int pageSize = 5)
        {
            try
            {
                var result = await _bookService.GetBookIndexAsync(searchTerm, page, pageSize);
                ViewData["SearchTerm"] = searchTerm;
                return View(result);
            }
            catch (Exception e)
            {

                Console.WriteLine($"An error has occured: {e.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id, int page = 1)
        {
            try
            {
                string? userId = this.GetUserId();
                BookDetailsViewModel? bookDetailsVM = await this._bookService.GetBookDetailsAsync(id, userId, page);
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
            catch (Exception e)
            {

                Console.WriteLine($"An error has occured: {e.Message}");
                return RedirectToAction(nameof(Create));
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
                bool createResult = await this._bookService.CreateBookAsync(this.GetUserId()!, inputModel);

                if (createResult == false) 
                {
                    ModelState.AddModelError(string.Empty, "A fatal error has occured. Please try again later.");
                    inputModel.Genres = await this._genreService.GetAllAsSelectListAsync();
                    return View(inputModel);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error has occured: {e.Message}");
                return RedirectToAction(nameof(Create));
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                string userId = this.GetUserId()!;

                BookEditInputModel? editInputModel = await this._bookService.GetBookForEditingAsync(userId, id.Value);
                if(editInputModel == null)
                {
                    return RedirectToAction(nameof(Edit));
                }
                editInputModel.Genres = await this._genreService.GetAllAsSelectListAsync();
                return View(editInputModel);
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error has occured: {e.Message}");
                return RedirectToAction(nameof(Edit));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(BookEditInputModel editInputModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    editInputModel.Genres = await this._genreService.GetAllAsSelectListAsync();
                    return View(editInputModel);
                }
                bool editResult = await this._bookService.UpdateEditedBookAsync(GetUserId()!, editInputModel);
                if (editResult == false)
                {
                    ModelState.AddModelError(string.Empty, "A fatal error has occured. Please try again later.");

                    editInputModel.Genres = await this._genreService.GetAllAsSelectListAsync();
                    return View(editInputModel);
                }
                return RedirectToAction(nameof(Details), new { id = editInputModel.Id });
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error has occured: {e.Message}");
                return RedirectToAction(nameof(Edit));
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                string userId = this.GetUserId()!;
                BookDeleteInputModel? bookDeleteInputModel = await this._bookService.GetBookForDeletingAsync(userId, id);

                if (bookDeleteInputModel == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View(bookDeleteInputModel);
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error has occured: {e.Message}");
                return RedirectToAction(nameof(Delete));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmDelete(BookDeleteInputModel inputModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Invalid data provided. Please try again.");
                    return View(inputModel);
                }

                bool deleteResult = await this._bookService.SoftDeleteBookAsync(this.GetUserId()!, inputModel);
                if (deleteResult == false)
                {
                    ModelState.AddModelError(string.Empty, "A fatal error has occured. Please try again later.");
                    return View(inputModel);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error has occured: {e.Message}");
                return RedirectToAction(nameof(Delete));
            }
        }
    }
}
