namespace LibraryManagement.Services.Core.Tests.Controllers.UserControllerTests
{
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.Controllers;
    using LibraryManagement.Web.ViewModels.User;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System.Security.Claims;
    using static LibraryManagement.GCommon.ErrorMessages;

    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IMembershipService> _membershipServiceMock;
        private Mock<ILogger<UserController>> _loggerMock;
        private UserController _controller;

        [SetUp]
        public void SetUp()
        {
            _membershipServiceMock = new Mock<IMembershipService>();
            _loggerMock = new Mock<ILogger<UserController>>();
            _controller = new UserController(_membershipServiceMock.Object, _loggerMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _controller.TempData = new TempDataDictionary(_controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
        }

        [TearDown]
        public void TearDown() => _controller.Dispose();

        private void FakeUser(string userId)
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "mock"));
            _controller.ControllerContext.HttpContext.User = principal;
        }

        [Test]
        public async Task Dashboard_ShouldReturnViewWithMembershipStatus()
        {

            var userId = "user123";
            var member = new Member { Status = MembershipStatus.Approved };

            FakeUser(userId);
            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ReturnsAsync(member);

            var result = await _controller.Dashboard();

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.InstanceOf<UserMembershipViewModel>());

            var model = (UserMembershipViewModel)viewResult.Model;
            Assert.That(model.MembershipStatus, Is.EqualTo(MembershipStatus.Approved));
        }

        [Test]
        public async Task Dashboard_WhenExceptionThrown_ShouldLogErrorAndReturnErrorView()
        {

            var userId = "user123";
            FakeUser(userId);

            _membershipServiceMock
                .Setup(s => s.GetMembershipByUserIdAsync(userId))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _controller.Dashboard();

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewName, Is.EqualTo("Error"));
            Assert.That(_controller.TempData["ErrorMessage"], Is.EqualTo(UnexpectedErrorMessage));

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(UserDashboardErrorMessage)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }
    }
}
