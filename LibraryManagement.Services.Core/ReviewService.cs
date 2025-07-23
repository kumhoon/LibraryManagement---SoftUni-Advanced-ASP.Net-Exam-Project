namespace LibraryManagement.Services.Core
{
    using LibraryManagement.Data.Interfaces;
    using LibraryManagement.Data.Models;
    using LibraryManagement.Services.Common;
    using LibraryManagement.Services.Core.Interfaces;
    using LibraryManagement.Web.ViewModels.Review;
    using static LibraryManagement.GCommon.Defaults.Text;
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMembershipRepository _memberRepository;

        public ReviewService(IReviewRepository reviewRepository, IBookRepository bookRepository, IMembershipRepository memberRepository)
        {
            _bookRepository = bookRepository;
            _reviewRepository = reviewRepository;
            _memberRepository = memberRepository;
        }

        public async Task<bool> CreateReviewAsync(Guid bookId, Guid memberId, int rating, string? content)
        {
            if (bookId == Guid.Empty || memberId == Guid.Empty || rating < 1 || rating > 5) return false;


            var existing = await _reviewRepository
                .FirstOrDefaultAsync(r => r.BookId == bookId && r.MemberId == memberId);

            if (existing != null) return false;

            Review review = new()
            {
                Id = Guid.NewGuid(),
                MemberId = memberId,
                BookId = bookId,
                Rating = rating,
                Content = content,
                CreatedAt = DateTime.UtcNow,
            };

            await _reviewRepository.AddAsync(review);
            return true;
        }

        public async Task<bool> UpdateReviewAsync(Guid memberId, Guid bookId, int rating, string? content)
        {
            if (bookId == Guid.Empty || memberId == Guid.Empty || rating < 1 || rating > 5) return false;

            Review? review = await _reviewRepository
                .FirstOrDefaultAsync(r => r.BookId == bookId && r.MemberId == memberId);

            if (review == null || memberId != review.MemberId) return false;

            review.Rating = rating;
            review.Content = content;
            review.IsApproved = false;

            return await _reviewRepository.UpdateAsync(review);
        }

        public async Task<ReviewInputModel?> GetMemberReviewForBookAsync(Guid memberId, Guid bookId)
        {
            if (memberId == Guid.Empty || bookId == Guid.Empty) return null;

            Review? review = await _reviewRepository
                .FirstOrDefaultAsync(r => r.BookId == bookId && r.MemberId == memberId);

            if (review == null) return null;

            return new ReviewInputModel
            {
                BookId = bookId,
                ReviewId = review.Id,
                Rating = review.Rating,
                Content = review.Content
            };
        }

        public async Task<BookReviewsViewModel> GetBookReviewsAsync(Guid bookId, int pageNumber, int pageSize)
        {         
            var allApproved = await _reviewRepository.GetApprovedByBookAsync(bookId);

            double averageRating = allApproved.Any()
                ? allApproved.Average(r => r.Rating)
                : 0.0;

            int totalReviews = allApproved.Count();
            
            var pagedReviews = allApproved
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var display = new List<ReviewDisplayInputModel>(pagedReviews.Count);
            foreach (var r in pagedReviews)
            {
                var member = await _memberRepository.GetByIdAsync(r.MemberId);

                display.Add(new ReviewDisplayInputModel
                {
                    MemberName = member?.Name ?? UnknownMember,
                    Rating = r.Rating,
                    Content = r.Content,
                    CreatedAt = r.CreatedAt
                });
            }

            var pagedResult = new PagedResult<ReviewDisplayInputModel>
            {
                Items = display,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalReviews
            };

            return new BookReviewsViewModel
            {
                BookId = bookId,
                AverageRating = Math.Round(averageRating, 2),
                TotalReviews = totalReviews,
                Reviews = pagedResult
            };
        }

        public async Task<IEnumerable<PendingReviewViewModel>> GetPendingReviewsAsync()
        {
            IEnumerable<Review> pending = await _reviewRepository.GetPendingAsync();
            List<PendingReviewViewModel> result = new(pending.Count());

            foreach (var r in pending)
            {
                Member? member = await _memberRepository.GetByIdAsync(r.MemberId);
                Book? book = await _bookRepository.GetByIdAsync(r.BookId);
                result.Add(new PendingReviewViewModel
                {
                    ReviewId = r.Id,
                    BookId = r.BookId,
                    BookTitle = book?.Title ?? UnknownTitle,
                    MemberId = r.MemberId,
                    MemberName = member?.Name ?? UnknownMember,
                    Rating = r.Rating,
                    Content = r.Content,
                    CreatedAt = r.CreatedAt
                });
            }
            return result;

        }

        public Task<bool> ApproveReviewAsync(Guid reviewId)
            => _reviewRepository.ApproveAsync(reviewId);

        public Task<bool> RejectReviewAsync(Guid reviewId)
            => _reviewRepository.RejectAsync(reviewId);
    }
}
