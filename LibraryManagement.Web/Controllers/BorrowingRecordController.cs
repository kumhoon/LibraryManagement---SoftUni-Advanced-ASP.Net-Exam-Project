namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.BorrowingRecord;
    using Microsoft.AspNetCore.Mvc;
    using static LibraryManagement.GCommon.Messages.BorrowingRecordMessages;
    using static LibraryManagement.GCommon.Defaults.Text;
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

            
            bool hasActiveBorrow = await _borrowingRecordService.HasAnyActiveBorrowAsync(member.Id);
            if (hasActiveBorrow)
            {
                var book = await _bookService.GetBookByIdAsync(bookId);
                var vm = new BorrowingRecordResultViewModel
                {
                    Success = false,
                    Message = BorrowLimitExceeded,
                    BookTitle = book?.Title ?? UnknownTitle
                };
                return View("BorrowResult", vm);
            }

            
            var borrowResult = await _borrowingRecordService.BorrowBookAsync(member.Id, bookId);
            var borrowedBook = await _bookService.GetBookByIdAsync(bookId);

            string message = borrowResult switch
            {
                BorrowResult.Success => BorrowSuccess,
                BorrowResult.AlreadyBorrowedByMember => BookAlreadyBorrowedByMember,
                BorrowResult.BookUnavailable => BookAlreadyBorrowedByAnotherMember,
                _ => BorrowFailed
            };

            var resultVm = new BorrowingRecordResultViewModel
            {
                Success = borrowResult == BorrowResult.Success,
                Message = message,
                BookTitle = borrowedBook?.Title ?? UnknownTitle
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
                BookTitle = book?.Title ?? UnknownTitle,
                Message = result ? ReturnSuccess : ReturnFailed
            };

            return View("ReturnResult", viewModel);
        }

    }
}
