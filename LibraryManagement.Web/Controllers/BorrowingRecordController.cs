namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.BorrowingRecord;
    using Microsoft.AspNetCore.Mvc;

    public class BorrowingRecordController : BaseController
    {
        private readonly IBorrowingRecordService _borrowingRecordService;
        private readonly IMembershipService _membershipService;
        private readonly IBookService _bookService;

        public BorrowingRecordController(IBorrowingRecordService borrowingRecordService, IMembershipService membershipService, IBookService bookService)
        {
            _borrowingRecordService = borrowingRecordService;
            _membershipService = membershipService;
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = this.GetUserId()!;
            var member = await _membershipService.GetMembershipByUserIdAsync(userId);

            if (member == null)
            {
                
                return RedirectToAction("Apply", "Membership");
            }

            var history = await _borrowingRecordService.GetBorrowingHistoryAsync(member.Id);
            return View(history);
        }

        [HttpPost]
        public async Task<IActionResult> BorrowBook(Guid bookId)
        {
            var userId = this.GetUserId()!;
            var member = await _membershipService.GetMembershipByUserIdAsync(userId);

            if (member == null)
            {
                return RedirectToAction("Apply", "Membership");
            }

            var result = await _borrowingRecordService.BorrowBookAsync(member.Id, bookId);

            var book = await _bookService.GetBookByIdAsync(bookId); 

            var vm = new BorrowingRecordResultViewModel
            {
                Success = result,
                Message = result ? "Book successfully borrowed!" : "Failed to borrow the book.",
                BookTitle = book?.Title ?? "Unknown Book"
            };

            return View("BorrowResult", vm);
        }

        [HttpPost]
        public async Task<IActionResult> ReturnBook(Guid bookId)
        {
            var userId = this.GetUserId()!;
            var member = await _membershipService.GetMembershipByUserIdAsync(userId);

            if (member == null)
            {
                return RedirectToAction("Apply", "Membership");
            }

            var result = await _borrowingRecordService.ReturnBookAsync(member.Id, bookId);

            var book = await _bookService.GetBookByIdAsync(bookId); 

            var viewModel = new BorrowingRecordResultViewModel
            {
                Success = result,
                BookTitle = book?.Title ?? "Unknown Book",
                Message = result ? "Book returned successfully." : "Failed to return the book."
            };

            return View("ReturnResult", viewModel);
        }

    }
}
