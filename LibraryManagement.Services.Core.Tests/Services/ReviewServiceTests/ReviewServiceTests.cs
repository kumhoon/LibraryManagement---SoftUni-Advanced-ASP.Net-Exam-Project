using LibraryManagement.Data.Interfaces;
using LibraryManagement.Data.Models;
using Moq;
using System.Linq.Expressions;

namespace LibraryManagement.Services.Core.Tests.Services.ReviewServiceTests
{
    [TestFixture]
    public class ReviewServiceTests
    {
        private Mock<IReviewRepository> _reviewRepositoryMock;
        private Mock<IBookRepository> _bookRepositoryMock;
        private Mock<IMembershipRepository> _memberRepositoryMock;
        private ReviewService _reviewService;

        [SetUp]
        public void SetUp()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _bookRepositoryMock = new Mock<IBookRepository>();
            _memberRepositoryMock = new Mock<IMembershipRepository>();
            _reviewService = new ReviewService(_reviewRepositoryMock.Object, _bookRepositoryMock.Object, _memberRepositoryMock.Object);
        }

        [TestCase(0, true)] 
        [TestCase(6, true)] 
        public async Task CreateReviewAsync_InvalidRating_ReturnsFalse(int invalidRating, bool _)
        {

            var result = await _reviewService.CreateReviewAsync(Guid.NewGuid(), Guid.NewGuid(), invalidRating, "Bad rating");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CreateReviewAsync_EmptyBookId_ReturnsFalse()
        {

            var result = await _reviewService.CreateReviewAsync(Guid.Empty, Guid.NewGuid(), 3, "Test");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CreateReviewAsync_EmptyMemberId_ReturnsFalse()
        {

            var result = await _reviewService.CreateReviewAsync(Guid.NewGuid(), Guid.Empty, 3, "Test");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CreateReviewAsync_ReviewAlreadyExists_ReturnsFalse()
        {

            var bookId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            _reviewRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Review, bool>>>()))
                .ReturnsAsync(new Review());

            var result = await _reviewService.CreateReviewAsync(bookId, memberId, 5, "Already reviewed");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CreateReviewAsync_ValidInput_AddsReviewAndReturnsTrue()
        {

            var bookId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            _reviewRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Review, bool>>>()))
                .ReturnsAsync((Review?)null);

            _reviewRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Review>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var result = await _reviewService.CreateReviewAsync(bookId, memberId, 4, "Good book!");

            Assert.That(result, Is.True);
            _reviewRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Once);
        }

        [TestCase(0)]
        [TestCase(6)]
        public async Task UpdateReviewAsync_InvalidRating_ReturnsFalse(int invalidRating)
        {
            var result = await _reviewService.UpdateReviewAsync(Guid.NewGuid(), Guid.NewGuid(), invalidRating, "Invalid rating");
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateReviewAsync_EmptyBookId_ReturnsFalse()
        {
            var result = await _reviewService.UpdateReviewAsync(Guid.NewGuid(), Guid.Empty, 4, "Missing book");
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateReviewAsync_EmptyMemberId_ReturnsFalse()
        {
            var result = await _reviewService.UpdateReviewAsync(Guid.Empty, Guid.NewGuid(), 4, "Missing member");
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateReviewAsync_ReviewNotFound_ReturnsFalse()
        {
            _reviewRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Review, bool>>>()))
                .ReturnsAsync((Review?)null);

            var result = await _reviewService.UpdateReviewAsync(Guid.NewGuid(), Guid.NewGuid(), 4, "No review exists");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateReviewAsync_ReviewMemberMismatch_ReturnsFalse()
        {
            var memberId = Guid.NewGuid();
            var review = new Review
            {
                MemberId = Guid.NewGuid() 
            };

            _reviewRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Review, bool>>>()))
                .ReturnsAsync(review);

            var result = await _reviewService.UpdateReviewAsync(memberId, Guid.NewGuid(), 4, "Mismatch");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateReviewAsync_ValidUpdate_ReturnsTrue()
        {
            var memberId = Guid.NewGuid();
            var review = new Review
            {
                MemberId = memberId,
                Rating = 2,
                Content = "Old content",
                IsApproved = true
            };

            _reviewRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Review, bool>>>()))
                .ReturnsAsync(review);

            _reviewRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Review>()))
                .ReturnsAsync(true)
                .Verifiable();

            var result = await _reviewService.UpdateReviewAsync(memberId, Guid.NewGuid(), 5, "Updated content");

            Assert.That(result, Is.True);
            Assert.That(review.Rating, Is.EqualTo(5));
            Assert.That(review.Content, Is.EqualTo("Updated content"));
            Assert.That(review.IsApproved, Is.False);
            _reviewRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Review>()), Times.Once);
        }

        [Test]
        public async Task GetMemberReviewForBookAsync_EmptyMemberId_ReturnsNull()
        {
            var result = await _reviewService.GetMemberReviewForBookAsync(Guid.Empty, Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetMemberReviewForBookAsync_EmptyBookId_ReturnsNull()
        {
            var result = await _reviewService.GetMemberReviewForBookAsync(Guid.NewGuid(), Guid.Empty);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetMemberReviewForBookAsync_ReviewNotFound_ReturnsNull()
        {
            _reviewRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Review, bool>>>()))
                .ReturnsAsync((Review?)null);

            var result = await _reviewService.GetMemberReviewForBookAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetMemberReviewForBookAsync_ReviewFound_ReturnsReviewInputModel()
        {
            var memberId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();

            var review = new Review
            {
                Id = reviewId,
                MemberId = memberId,
                BookId = bookId,
                Rating = 4,
                Content = "Nice book"
            };

            _reviewRepositoryMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Review, bool>>>()))
                .ReturnsAsync(review);

            var result = await _reviewService.GetMemberReviewForBookAsync(memberId, bookId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ReviewId, Is.EqualTo(reviewId));
            Assert.That(result.BookId, Is.EqualTo(bookId));
            Assert.That(result.Rating, Is.EqualTo(4));
            Assert.That(result.Content, Is.EqualTo("Nice book"));
        }

        [Test]
        public async Task GetBookReviewsAsync_WithValidData_ReturnsCorrectViewModel()
        {

            var bookId = Guid.NewGuid();
            int pageNumber = 1;
            int pageSize = 5;

            var memberId = Guid.NewGuid();

            var reviews = new List<Review>
            {
                new Review { BookId = bookId, MemberId = memberId, Rating = 4, CreatedAt = DateTime.UtcNow.AddDays(-2), Content = "Great" },
                new Review { BookId = bookId, MemberId = memberId, Rating = 5, CreatedAt = DateTime.UtcNow.AddDays(-1), Content = "Excellent" },
                new Review { BookId = bookId, MemberId = memberId, Rating = 3, CreatedAt = DateTime.UtcNow.AddDays(-3), Content = "Okay" },
                new Review { BookId = bookId, MemberId = memberId, Rating = 2, CreatedAt = DateTime.UtcNow.AddDays(-3), Content = "Not good" },
                new Review { BookId = bookId, MemberId = memberId, Rating = 1, CreatedAt = DateTime.UtcNow.AddDays(-3), Content = "Bad" }
            };

            var member = new Member { Id = memberId, Name = "John Doe" };

            _reviewRepositoryMock
                .Setup(r => r.GetApprovedByBookAsync(bookId))
                .ReturnsAsync(reviews);

            _memberRepositoryMock
                .Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(member);

            var result = await _reviewService.GetBookReviewsAsync(bookId, pageNumber, pageSize);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.BookId, Is.EqualTo(bookId));
            Assert.That(result.TotalReviews, Is.EqualTo(5));
            Assert.That(result.AverageRating, Is.EqualTo(Math.Round(reviews.Average(r => r.Rating), 2)));

            var items = result.Reviews.Items.ToList(); 
            Assert.That(items.Count, Is.EqualTo(pageSize));
            Assert.That(items[0].Content, Is.EqualTo("Excellent")); 
            Assert.That(items[1].Content, Is.EqualTo("Great"));
        }

        [Test]
        public async Task GetBookReviewsAsync_WithNoReviews_ReturnsEmptyList()
        {

            var bookId = Guid.NewGuid();
            int pageNumber = 1;
            int pageSize = 5;

            _reviewRepositoryMock
                .Setup(r => r.GetApprovedByBookAsync(bookId))
                .ReturnsAsync(new List<Review>());

            var result = await _reviewService.GetBookReviewsAsync(bookId, pageNumber, pageSize);

            Assert.That(result.TotalReviews, Is.EqualTo(0));
            Assert.That(result.AverageRating, Is.EqualTo(0));
            Assert.That(result.Reviews.Items.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetPendingReviewsAsync_WithValidPendingReviews_ReturnsExpectedViewModels()
        {

            var review1 = new Review
            {
                Id = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid(),
                Rating = 5,
                Content = "Excellent book",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            var review2 = new Review
            {
                Id = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid(),
                Rating = 3,
                Content = "It was okay",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            };

            var pendingReviews = new List<Review> { review1, review2 };

            var member1 = new Member { Id = review1.MemberId, Name = "John Doe" };
            var member2 = new Member { Id = review2.MemberId, Name = "Jane Smith" };

            var book1 = new Book { Id = review1.BookId, Title = "Book One" };
            var book2 = new Book { Id = review2.BookId, Title = "Book Two" };

            _reviewRepositoryMock
                .Setup(r => r.GetPendingAsync())
                .ReturnsAsync(pendingReviews);

            _memberRepositoryMock
                .Setup(m => m.GetByIdAsync(review1.MemberId))
                .ReturnsAsync(member1);
            _memberRepositoryMock
                .Setup(m => m.GetByIdAsync(review2.MemberId))
                .ReturnsAsync(member2);

            _bookRepositoryMock
                .Setup(b => b.GetByIdAsync(review1.BookId))
                .ReturnsAsync(book1);
            _bookRepositoryMock
                .Setup(b => b.GetByIdAsync(review2.BookId))
                .ReturnsAsync(book2);

            var result = (await _reviewService.GetPendingReviewsAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(result[0].ReviewId, Is.EqualTo(review1.Id));
            Assert.That(result[0].BookId, Is.EqualTo(review1.BookId));
            Assert.That(result[0].BookTitle, Is.EqualTo(book1.Title));
            Assert.That(result[0].MemberId, Is.EqualTo(review1.MemberId));
            Assert.That(result[0].MemberName, Is.EqualTo(member1.Name));
            Assert.That(result[0].Rating, Is.EqualTo(review1.Rating));
            Assert.That(result[0].Content, Is.EqualTo(review1.Content));
            Assert.That(result[0].CreatedAt, Is.EqualTo(review1.CreatedAt));

            Assert.That(result[1].BookTitle, Is.EqualTo(book2.Title));
            Assert.That(result[1].MemberName, Is.EqualTo(member2.Name));
        }

        [Test]
        public async Task GetPendingReviewsAsync_WithMissingMemberOrBook_UsesFallbackValues()
        {

            var review = new Review
            {
                Id = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid(),
                Rating = 4,
                Content = "Interesting read",
                CreatedAt = DateTime.UtcNow
            };

            var pendingReviews = new List<Review> { review };

            _reviewRepositoryMock
                .Setup(r => r.GetPendingAsync())
                .ReturnsAsync(pendingReviews);

            _memberRepositoryMock
                .Setup(m => m.GetByIdAsync(review.MemberId))
                .ReturnsAsync((Member?)null);

            _bookRepositoryMock
                .Setup(b => b.GetByIdAsync(review.BookId))
                .ReturnsAsync((Book?)null);

            var result = (await _reviewService.GetPendingReviewsAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].BookTitle, Is.EqualTo("Unknown Title"));
            Assert.That(result[0].MemberName, Is.EqualTo("Unknown Member"));
            Assert.That(result[0].Rating, Is.EqualTo(4));
            Assert.That(result[0].Content, Is.EqualTo("Interesting read"));
        }

        [Test]
        public async Task ApproveReviewAsync_CallsRepositoryAndReturnsTrue()
        {

            var reviewId = Guid.NewGuid();
            _reviewRepositoryMock
                .Setup(r => r.ApproveAsync(reviewId))
                .ReturnsAsync(true);

            var result = await _reviewService.ApproveReviewAsync(reviewId);

            Assert.That(result, Is.True);
            _reviewRepositoryMock.Verify(r => r.ApproveAsync(reviewId), Times.Once);
        }

        [Test]
        public async Task ApproveReviewAsync_CallsRepositoryAndReturnsFalse()
        {
            var reviewId = Guid.NewGuid();
            _reviewRepositoryMock
                .Setup(r => r.ApproveAsync(reviewId))
                .ReturnsAsync(false);

            var result = await _reviewService.ApproveReviewAsync(reviewId);

            Assert.That(result, Is.False);
            _reviewRepositoryMock.Verify(r => r.ApproveAsync(reviewId), Times.Once);
        }

        [Test]
        public async Task RejectReviewAsync_CallsRepositoryAndReturnsTrue()
        {
            var reviewId = Guid.NewGuid();
            _reviewRepositoryMock
                .Setup(r => r.RejectAsync(reviewId))
                .ReturnsAsync(true);

            var result = await _reviewService.RejectReviewAsync(reviewId);

            Assert.That(result, Is.True);
            _reviewRepositoryMock.Verify(r => r.RejectAsync(reviewId), Times.Once);
        }

        [Test]
        public async Task RejectReviewAsync_CallsRepositoryAndReturnsFalse()
        {
            var reviewId = Guid.NewGuid();
            _reviewRepositoryMock
                .Setup(r => r.RejectAsync(reviewId))
                .ReturnsAsync(false);

            var result = await _reviewService.RejectReviewAsync(reviewId);

            Assert.That(result, Is.False);
            _reviewRepositoryMock.Verify(r => r.RejectAsync(reviewId), Times.Once);
        }
    }
}
