namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Web.ViewModels.Review;
    public interface IReviewService 
    {
        Task<bool> CreateReviewAsync(Guid memberId, Guid bookId, int rating, string? content);

        Task<bool> UpdateReviewAsync(Guid memberId, Guid reviewId, int rating, string? content);

        Task<ReviewInputModel?> GetMemberReviewForBookAsync(Guid memberId, Guid bookId);

        Task<BookReviewsViewModel> GetBookReviewsAsync(Guid bookId, int pageNumber, int pageSize);

        Task<IEnumerable<PendingReviewViewModel>> GetPendingReviewsAsync();

        Task<bool> ApproveReviewAsync(Guid reviewId);

        Task<bool> RejectReviewAsync(Guid reviewId);
    }
}
