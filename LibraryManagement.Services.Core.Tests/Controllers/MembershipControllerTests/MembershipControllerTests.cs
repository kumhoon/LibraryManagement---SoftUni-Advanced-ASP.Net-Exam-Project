
using LibraryManagement.Data.Models;
using LibraryManagement.Services.Core.Interfaces;
using LibraryManagement.Web.Controllers;
using LibraryManagement.Web.ViewModels.Membership;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using static LibraryManagement.GCommon.ErrorMessages;

namespace LibraryManagement.Services.Core.Tests.Controllers.MembershipControllerTests
{
    [TestFixture]
    public class MembershipControllerTests
    {
        private Mock<IMembershipService> _membershipServiceMock;
        private Mock<ILogger<MembershipController>> _loggerMock;
        private MembershipController _controller;

        [SetUp]
        public void SetUp()
        {
            _membershipServiceMock = new Mock<IMembershipService>();
            _loggerMock = new Mock<ILogger<MembershipController>>();
            _controller = new MembershipController(_membershipServiceMock.Object, _loggerMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            _controller.TempData = new TempDataDictionary(_controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
        }

        [TearDown]
        public void TearDown() => _controller.Dispose();

        private void FakeUser(string userId, string email = "alice@example.com")
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(
                new[] 
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, email)
                }, "mock"));
            _controller.ControllerContext.HttpContext.User = principal;
        }

        [Test]
        public async Task Apply_WhenNoMembership_CanApplyTrue()
        {

            FakeUser("user123");
            _membershipServiceMock
                      .Setup(s => s.GetMembershipByUserIdAsync("user123"))
                      .ReturnsAsync((Member)null!);

            var result = await _controller.Apply();

            var view = (ViewResult)result;
            Assert.That(view.ViewData["CanApply"], Is.True);
        }

        [Test]
        public async Task Apply_WhenApprovedMembership_CanApplyFalse()
        {
            FakeUser("user123");
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync("user123"))
                .ReturnsAsync(new Member { Status = MembershipStatus.Approved });

            var view = (ViewResult)await _controller.Apply();

            Assert.That(view.ViewData["CanApply"], Is.False);
        }

        [Test]
        public async Task Apply_WhenServiceThrows_ShouldRedirectAndSetTempDataError()
        {
            FakeUser("user123");
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync("user123"))
                .ThrowsAsync(new Exception("oops"));

            var result = await _controller.Apply();

            var redirect = (RedirectToActionResult)result;

            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Home"));
            Assert.That(_controller.TempData["ErrorMessage"], Is.EqualTo(UnexpectedErrorMessage));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(MembershipApplicationPageErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task ApplyPost_WhenModelStateInvalid_ShouldReturnViewWithInputModel()
        {

            var input = new MemberApplicationInputModel { Name = "alice" };
            _controller.ModelState.AddModelError("Name", "Required");

            var result = await _controller.Apply(input);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var view = (ViewResult)result;

            Assert.That(view.Model, Is.SameAs(input));
        }

        [Test]
        public async Task ApplyPost_WhenValid_ShouldCallServiceAndRedirect()
        {

            FakeUser("user123");
            var input = new MemberApplicationInputModel { Name = "alice" };

            _membershipServiceMock
                .Setup(s => s.ApplyForMembershipAsync("user123", input))
                .Returns(Task.CompletedTask);

            var result = await _controller.Apply(input);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;

            Assert.That(redirect.ActionName, Is.EqualTo(nameof(MembershipController.ApplyConfirmation)));
            _membershipServiceMock.Verify(s =>
                s.ApplyForMembershipAsync("user123", input), Times.Once);
        }

        [Test]
        public async Task ApplyPost_WhenServiceThrowsInvalidOp_ShouldAddModelErrorAndReturnView()
        {

            FakeUser("user123");
            var input = new MemberApplicationInputModel { Name = "alice" };
            var ex = new InvalidOperationException("Already applied");

            _membershipServiceMock
                .Setup(s => s.ApplyForMembershipAsync("user123", input))
                .ThrowsAsync(ex);

            var result = await _controller.Apply(input);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            Assert.That(_controller.ModelState[string.Empty]!.Errors.First().ErrorMessage,
                        Is.EqualTo(ex.Message));
        }

        [Test]
        public async Task ApplyPost_WhenExceptionThrown_ShouldLogErrorSetTempDataAndRedirectHomeIndex()
        {

            FakeUser("user123");
            var input = new MemberApplicationInputModel { Name = "alice" };
            var ex = new Exception("oops");

            _membershipServiceMock
                .Setup(s => s.ApplyForMembershipAsync("user123", input))
                .ThrowsAsync(ex);

            var result = await _controller.Apply(input);

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirect = (RedirectToActionResult)result;
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Home"));

            Assert.That(_controller.TempData["ErrorMessage"], Is.EqualTo(UnexpectedErrorMessage));
            _loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(MembershipApplicationErrorMessage)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }
    }
}
