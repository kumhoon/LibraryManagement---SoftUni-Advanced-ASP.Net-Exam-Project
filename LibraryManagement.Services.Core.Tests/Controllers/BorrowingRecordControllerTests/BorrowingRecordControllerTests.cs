namespace LibraryManagement.Services.Core.Tests.Controllers.BorrowingRecordControllerTests
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.Controllers;
    using LibraryManagement.Web.ViewModels.BorrowingRecord;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System.Security.Claims;
    using static LibraryManagement.GCommon.ErrorMessages;
    using static LibraryManagement.GCommon.Messages.BorrowingRecordMessages;

    [TestFixture]
    public class BorrowingRecordControllerTests
    {
        private Mock<IBorrowingRecordService> _borrowingRecordServiceMock;
        private Mock<IMembershipService> _membershipServiceMock;
        private Mock<IBookService> _bookServiceMock;
        private Mock<ILogger<BorrowingRecordController>> _loggerMock;
        private BorrowingRecordController _controller;
        [SetUp]
        public void Setup()
        {
            _borrowingRecordServiceMock = new Mock<IBorrowingRecordService>();
            _membershipServiceMock = new Mock<IMembershipService>();
            _bookServiceMock = new Mock<IBookService>();
            _loggerMock = new Mock<ILogger<BorrowingRecordController>>();
            _controller = new BorrowingRecordController(
                _borrowingRecordServiceMock.Object,
                _membershipServiceMock.Object,
                _bookServiceMock.Object,
                _loggerMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [TearDown]
        public void TearDown() => _controller.Dispose();

        private void FakeUser(string userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "mock"));
            _controller.ControllerContext.HttpContext.User = user;
        }

        [Test]
        public async Task History_WhenNoMembership_ShouldRedirectToMembershipApply()
        {

            const string userId = "user123";
            FakeUser(userId);

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync((Member)null!);

            var result = await _controller.History();

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo("Apply"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Membership"));
        }

        [Test]
        public async Task History_WhenMemberExists_ShouldReturnViewWithHistory()
        {

            const string userId = "user123";
            FakeUser(userId);

            var member = new Member { Id = Guid.NewGuid() };
            var history = new List<BorrowingRecordViewModel>
            {
                new BorrowingRecordViewModel { Title = "1984" }
            };

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(member);

            _borrowingRecordServiceMock
                .Setup(s => s.GetBorrowingHistoryAsync(member.Id))
                .ReturnsAsync(history);

            var result = await _controller.History();

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;

            Assert.That(view.Model, Is.AssignableTo<IEnumerable<BorrowingRecordViewModel>>());
            var model = (IEnumerable<BorrowingRecordViewModel>)view.Model;
            Assert.That(model, Has.Exactly(1).Items);
            Assert.That(model.First().Title, Is.EqualTo("1984"));
        }

        [Test]
        public async Task History_WhenExceptionThrown_ShouldLogErrorAndReturnErrorView()
        {

            const string userId = "user123";
            FakeUser(userId);

            var member = new Member { Id = Guid.NewGuid() };

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(member);

            _borrowingRecordServiceMock
                .Setup(s => s.GetBorrowingHistoryAsync(member.Id))
                .ThrowsAsync(new Exception("boom"));

            var result = await _controller.History();

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;
            Assert.That(view.ViewName, Is.EqualTo("Error"));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(BorrowingHistoryErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task BorrowBook_WhenNoMembership_ShouldRedirectToMembershipApply()
        {

            var bookId = Guid.NewGuid();
            FakeUser("user123");

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync("user123"))
                .ReturnsAsync((Member)null!);

            var result = await _controller.BorrowBook(bookId);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var r = (RedirectToActionResult)result;
            Assert.That(r.ActionName, Is.EqualTo("Apply"));
            Assert.That(r.ControllerName, Is.EqualTo("Membership"));
        }

        [Test]
        public async Task BorrowBook_WhenHasActiveBorrow_ShouldReturnBorrowResultViewLimitExceeded()
        {
 
            var bookId = Guid.NewGuid();
            FakeUser("user123");
            var member = new Member { Id = Guid.NewGuid() };

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync("user123"))
                .ReturnsAsync(member);

            _borrowingRecordServiceMock
                .Setup(s => s.HasAnyActiveBorrowAsync(member.Id))
                .ReturnsAsync(true);

            _bookServiceMock
                .Setup(s => s.GetBookByIdAsync(bookId))
                .ReturnsAsync(new Book { Title = "Moby Dick" });

            var result = await _controller.BorrowBook(bookId);

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;
            Assert.That(view.ViewName, Is.EqualTo("BorrowResult"));

            var vm = (BorrowingRecordResultViewModel)view.Model;
            Assert.That(vm.Success, Is.False);
            Assert.That(vm.Message, Is.EqualTo(BorrowLimitExceeded));
            Assert.That(vm.BookTitle, Is.EqualTo("Moby Dick"));
        }

        [Test]
        public async Task BorrowBook_WhenBorrowSucceeds_ShouldReturnBorrowResultViewSuccess()
        {

            var bookId = Guid.NewGuid();
            FakeUser("user123");
            var member = new Member { Id = Guid.NewGuid() };

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync("user123"))
                .ReturnsAsync(member);

            _borrowingRecordServiceMock
                .Setup(s => s.HasAnyActiveBorrowAsync(member.Id))
                .ReturnsAsync(false);

            _borrowingRecordServiceMock
                .Setup(s => s.BorrowBookAsync(member.Id, bookId))
                .ReturnsAsync(BorrowResult.Success);

            _bookServiceMock
                .Setup(s => s.GetBookByIdAsync(bookId))
                .ReturnsAsync(new Book { Title = "1984" });

            var result = await _controller.BorrowBook(bookId);

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;
            Assert.That(view.ViewName, Is.EqualTo("BorrowResult"));

            var vm = (BorrowingRecordResultViewModel)view.Model;
            Assert.That(vm.Success, Is.True);
            Assert.That(vm.Message, Is.EqualTo(BorrowSuccess));
            Assert.That(vm.BookTitle, Is.EqualTo("1984"));
        }

        [Test]
        public async Task BorrowBook_WhenAlreadyBorrowedByMember_ShouldReturnBorrowResultViewAlreadyBorrowed()
        {

            var bookId = Guid.NewGuid();
            FakeUser("user123");
            var member = new Member { Id = Guid.NewGuid() };

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync("user123"))
                .ReturnsAsync(member);

            _borrowingRecordServiceMock
                .Setup(s => s.HasAnyActiveBorrowAsync(member.Id))
                .ReturnsAsync(false);

            _borrowingRecordServiceMock
                .Setup(s => s.BorrowBookAsync(member.Id, bookId))
                .ReturnsAsync(BorrowResult.AlreadyBorrowedByMember);
            _bookServiceMock
                .Setup(s => s.GetBookByIdAsync(bookId))
                .ReturnsAsync(new Book { Title = "Hamlet" });

            var result = await _controller.BorrowBook(bookId);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var view = (ViewResult)result;

            var vm = (BorrowingRecordResultViewModel)view.Model;
            Assert.That(vm.Success, Is.False);
            Assert.That(vm.Message, Is.EqualTo(BookAlreadyBorrowedByMember));
            Assert.That(vm.BookTitle, Is.EqualTo("Hamlet"));
        }

        [Test]
        public async Task BorrowBook_WhenExceptionThrown_ShouldLogErrorAndReturnErrorView()
        {

            var bookId = Guid.NewGuid();
            FakeUser("user123");
            var member = new Member { Id = Guid.NewGuid() };

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync("user123"))
                .ReturnsAsync(member);

            _borrowingRecordServiceMock
                .Setup(s => s.HasAnyActiveBorrowAsync(member.Id))
                .ReturnsAsync(false);

            _borrowingRecordServiceMock
                .Setup(s => s.BorrowBookAsync(member.Id, bookId))
                .ThrowsAsync(new Exception("fail"));

            var result = await _controller.BorrowBook(bookId);

            Assert.IsInstanceOf<ViewResult>(result);
            var view = (ViewResult)result;
            Assert.That(view.ViewName, Is.EqualTo("Error"));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(UnexpectedErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task ReturnBook_NoMembership_ShouldRedirectToMembershipApply()
        {

            var bookId = Guid.NewGuid();
            FakeUser("user123");
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync("user123"))
                .ReturnsAsync((Member)null!);

            var result = await _controller.ReturnBook(bookId);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo("Apply"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Membership"));
        }

        [Test]
        public async Task ReturnBook_WhenReturnSucceeds_ShouldReturnReturnResultViewSuccess()
        {

            var bookId = Guid.NewGuid();
            FakeUser("user123");

            var member = new Member { Id = Guid.NewGuid() };

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync("user123"))
                .ReturnsAsync(member);

            _borrowingRecordServiceMock
                .Setup(s => s.ReturnBookAsync(member.Id, bookId))
                .ReturnsAsync(true);

            _bookServiceMock
                .Setup(s => s.GetBookByIdAsync(bookId))
                .ReturnsAsync(new Book { Title = "Dune" });

            var result = await _controller.ReturnBook(bookId);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var view = (ViewResult)result;
            Assert.That(view.ViewName, Is.EqualTo("ReturnResult"));

            var vm = (BorrowingRecordResultViewModel)view.Model;
            Assert.That(vm.Success, Is.True);
            Assert.That(vm.Message, Is.EqualTo(ReturnSuccess));
            Assert.That(vm.BookTitle, Is.EqualTo("Dune"));
        }

        [Test]
        public async Task ReturnBook_WhenReturnFails_ShouldReturnReturnResultViewFailure()
        {

            var bookId = Guid.NewGuid();
            FakeUser("user123");
            var member = new Member { Id = Guid.NewGuid() };
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync("user123"))
                .ReturnsAsync(member);
            _borrowingRecordServiceMock
                .Setup(s => s.ReturnBookAsync(member.Id, bookId))
                .ReturnsAsync(false);
            _bookServiceMock
                .Setup(s => s.GetBookByIdAsync(bookId))
                .ReturnsAsync(new Book { Title = "Brave New World" });

            var result = await _controller.ReturnBook(bookId);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var view = (ViewResult)result;
            Assert.That(view.ViewName, Is.EqualTo("ReturnResult"));

            var vm = (BorrowingRecordResultViewModel)view.Model;
            Assert.That(vm.Success, Is.False);
            Assert.That(vm.Message, Is.EqualTo(ReturnFailed));
            Assert.That(vm.BookTitle, Is.EqualTo("Brave New World"));
        }

        [Test]
        public async Task ReturnBook_WhenKeyNotFound_ShouldReturnNotFound()
        {

            var bookId = Guid.NewGuid();
            FakeUser("user123");
            var member = new Member { Id = Guid.NewGuid() };
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync("user123"))
                .ReturnsAsync(member);
            _borrowingRecordServiceMock
                .Setup(s => s.ReturnBookAsync(member.Id, bookId))
                .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.ReturnBook(bookId);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task ReturnBook_WhenUnauthorized_ShouldReturnForbid()
        {

            var bookId = Guid.NewGuid();
            FakeUser("user123");
            var member = new Member { Id = Guid.NewGuid() };
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync("user123"))
                .ReturnsAsync(member);
            _borrowingRecordServiceMock
                .Setup(s => s.ReturnBookAsync(member.Id, bookId))
                .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.ReturnBook(bookId);

            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public async Task ReturnBook_WhenExceptionThrown_ShouldLogErrorAndReturnErrorView()
        {

            var bookId = Guid.NewGuid();
            FakeUser("user123");
            var member = new Member { Id = Guid.NewGuid() };
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync("user123"))
                .ReturnsAsync(member);
            _borrowingRecordServiceMock
                .Setup(s => s.ReturnBookAsync(member.Id, bookId))
                .ThrowsAsync(new Exception("oops"));

            var result = await _controller.ReturnBook(bookId);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var view = (ViewResult)result;
            Assert.That(view.ViewName, Is.EqualTo("Error"));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(UnexpectedErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }
    }
}
