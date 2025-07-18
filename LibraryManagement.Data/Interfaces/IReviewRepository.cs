namespace LibraryManagement.Data.Interfaces
{
    using LibraryManagement.Data.Models;
    public interface IReviewRepository : IRepository<Review, Guid>
    {
        Task<IEnumerable<Review>> GetApprovedByBookAsync(Guid bookId);

        Task<IEnumerable<Review>> GetAllForBookAsync(Guid bookId);

        Task<IEnumerable<Review>> GetByMemberAsync(Guid memberId);

        Task<IEnumerable<Review>> GetPendingAsync();

        Task<bool> ApproveAsync(Guid reviewId);

        Task<bool> RejectAsync(Guid reviewId);
    }
}
