namespace LibraryManagement.Data.Interfaces
{
    using LibraryManagement.Data.Models;

    /// <summary>
    /// Defines data access methods for managing book reviews.
    /// </summary>
    public interface IReviewRepository : IRepository<Review, Guid>
    {
        /// <summary>
        /// Retrieves all approved reviews for a specific book.
        /// </summary>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <returns>
        /// A collection of approved reviews for the given book.
        /// </returns>
        Task<IEnumerable<Review>> GetApprovedByBookAsync(Guid bookId);

        /// <summary>
        /// Retrieves all reviews for a specific book, regardless of approval status.
        /// </summary>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <returns>
        /// A collection of all reviews for the given book.
        /// </returns>
        Task<IEnumerable<Review>> GetAllForBookAsync(Guid bookId);

        /// <summary>
        /// Retrieves all reviews submitted by a specific member.
        /// </summary>
        /// <param name="memberId">The unique identifier of the member.</param>
        /// <returns>
        /// A collection of reviews authored by the specified member.
        /// </returns>
        Task<IEnumerable<Review>> GetByMemberAsync(Guid memberId);

        /// <summary>
        /// Retrieves all reviews that are pending approval.
        /// </summary>
        /// <returns>
        /// A collection of reviews awaiting moderation.
        /// </returns>
        Task<IEnumerable<Review>> GetPendingAsync();

        /// <summary>
        /// Approves a review, marking it as accepted.
        /// </summary>
        /// <param name="reviewId">The unique identifier of the review to approve.</param>
        /// <returns>
        /// <c>true</c> if the review was successfully approved; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ApproveAsync(Guid reviewId);

        /// <summary>
        /// Rejects a review, marking it as declined or removing it.
        /// </summary>
        /// <param name="reviewId">The unique identifier of the review to reject.</param>
        /// <returns>
        /// <c>true</c> if the review was successfully rejected; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> RejectAsync(Guid reviewId);
    }
}
