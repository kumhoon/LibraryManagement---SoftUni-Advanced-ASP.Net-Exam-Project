namespace LibraryManagement.Web.ViewModels.Review
{
    public class PendingReviewViewModel
    {
        public Guid ReviewId { get; set; }

        public Guid BookId { get; set; }

        public string BookTitle { get; set; } = null!;

        public Guid MemberId { get; set; }

        public string MemberName { get; set; } = null!;

        public int Rating { get; set; }

        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
