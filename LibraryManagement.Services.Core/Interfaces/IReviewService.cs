namespace LibraryManagement.Services.Core.Interfaces
{
    using LibraryManagement.Web.ViewModels.Review;

    /// <summary>
    /// Provides operations for creating, updating, retrieving, and moderating book reviews.
    /// </summary>
    public interface IReviewService 
    {
        /// <summary>
        /// Creates a new review for a specific book by a member.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member creating the review.</param>
        /// <param name="bookId">The unique identifier of the book being reviewed.</param>
        /// <param name="rating">The rating given to the book (e.g., 1–5).</param>
        /// <param name="content">Optional text content of the review.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result is <c>true</c> if the review was successfully created; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> CreateReviewAsync(Guid memberId, Guid bookId, int rating, string? content);

        /// <summary>
        /// Updates an existing review created by a member.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member updating the review.</param>
        /// <param name="reviewId">The unique identifier of the review to update.</param>
        /// <param name="rating">The updated rating value.</param>
        /// <param name="content">Optional updated text content of the review.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result is <c>true</c> if the review was successfully updated; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> UpdateReviewAsync(Guid memberId, Guid reviewId, int rating, string? content);

        /// <summary>
        /// Retrieves a review that a specific member has written for a specific book.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member.</param>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains the <see cref="ReviewInputModel"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<ReviewInputModel?> GetMemberReviewForBookAsync(Guid memberId, Guid bookId);

        /// <summary>
        /// Retrieves paginated reviews for a specific book.
        /// </summary>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of reviews per page.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains a <see cref="BookReviewsViewModel"/> with paginated review data.
        /// </returns>
        Task<BookReviewsViewModel> GetBookReviewsAsync(Guid bookId, int pageNumber, int pageSize);

        /// <summary>
        /// Retrieves reviews that are pending approval.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result contains a collection of <see cref="PendingReviewViewModel"/> objects  
        /// representing reviews awaiting moderation.
        /// </returns>
        Task<IEnumerable<PendingReviewViewModel>> GetPendingReviewsAsync();

        /// <summary>
        /// Approves a pending review, making it visible to users.
        /// </summary>
        /// <param name="reviewId">The unique identifier of the review to approve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result is <c>true</c> if the review was successfully approved; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ApproveReviewAsync(Guid reviewId);

        /// <summary>
        /// Rejects a pending review, preventing it from being displayed to users.
        /// </summary>
        /// <param name="reviewId">The unique identifier of the review to reject.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.  
        /// The task result is <c>true</c> if the review was successfully rejected; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> RejectReviewAsync(Guid reviewId);
    }
}
