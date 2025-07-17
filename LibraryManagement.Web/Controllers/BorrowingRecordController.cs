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

            // Check if member has any active borrow (one book at a time rule)
            bool hasActiveBorrow = await _borrowingRecordService.HasAnyActiveBorrowAsync(member.Id);
            if (hasActiveBorrow)
            {
                var book = await _bookService.GetBookByIdAsync(bookId);
                var vm = new BorrowingRecordResultViewModel
                {
                    Success = false,
                    Message = "You can only borrow one book at a time. Please return your current book before borrowing another.",
                    BookTitle = book?.Title ?? "Unknown Book"
                };
                return View("BorrowResult", vm);
            }

            // Try borrowing the book
            var borrowResult = await _borrowingRecordService.BorrowBookAsync(member.Id, bookId);
            var borrowedBook = await _bookService.GetBookByIdAsync(bookId);

            string message = borrowResult switch
            {
                BorrowResult.Success => "Book successfully borrowed!",
                BorrowResult.AlreadyBorrowedByMember => "You have already borrowed this book.",
                BorrowResult.BookUnavailable => "This book is currently borrowed by another user.",
                _ => "Failed to borrow the book."
            };

            var resultVm = new BorrowingRecordResultViewModel
            {
                Success = borrowResult == BorrowResult.Success,
                Message = message,
                BookTitle = borrowedBook?.Title ?? "Unknown Book"
            };

            return View("BorrowResult", resultVm);
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
