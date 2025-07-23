namespace LibraryManagement.Web.Controllers
{
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.BorrowingRecord;
    using Microsoft.AspNetCore.Mvc;
    using static LibraryManagement.GCommon.Messages.BorrowingRecordMessages;
    using static LibraryManagement.GCommon.Defaults.Text;
    using static LibraryManagement.GCommon.ErrorMessages;
    public class BorrowingRecordController : BaseController
    {
        private readonly IBorrowingRecordService _borrowingRecordService;
        private readonly IMembershipService _membershipService;
        private readonly IBookService _bookService;
        private readonly ILogger<BorrowingRecordController> _logger;

        public BorrowingRecordController(IBorrowingRecordService borrowingRecordService, IMembershipService membershipService, IBookService bookService, ILogger<BorrowingRecordController> logger)
        {
            _borrowingRecordService = borrowingRecordService;
            _membershipService = membershipService;
            _bookService = bookService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, BorrowingHistoryErrorMessage);
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> BorrowBook(Guid bookId)
        {
            try
            {
                var userId = GetUserId()!;
                var member = await _membershipService.GetMembershipByUserIdAsync(userId);

                if (member == null)
                {
                    return RedirectToAction("Apply", "Membership");
                }

                if (await _borrowingRecordService.HasAnyActiveBorrowAsync(member.Id))
                {
                    var book = await _bookService.GetBookByIdAsync(bookId);
                    return View("BorrowResult", new BorrowingRecordResultViewModel
                    {
                        Success = false,
                        Message = BorrowLimitExceeded,
                        BookTitle = book?.Title ?? UnknownTitle
                    });
                }

                var borrowResult = await _borrowingRecordService.BorrowBookAsync(member.Id, bookId);
                var bookDetails = await _bookService.GetBookByIdAsync(bookId);

                string message = borrowResult switch
                {
                    BorrowResult.Success => BorrowSuccess,
                    BorrowResult.AlreadyBorrowedByMember => BookAlreadyBorrowedByMember,
                    BorrowResult.BookUnavailable => BookAlreadyBorrowedByAnotherMember,
                    _ => BorrowFailed
                };

                return View("BorrowResult", new BorrowingRecordResultViewModel
                {
                    Success = borrowResult == BorrowResult.Success,
                    Message = message,
                    BookTitle = bookDetails?.Title ?? UnknownTitle
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, UnexpectedErrorMessage);
                return View("Error"); 
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReturnBook(Guid bookId)
        {
            try
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

    }
}
