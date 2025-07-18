namespace LibraryManagement.Web.ViewModels.Review
{
    public class ReviewDisplayViewModel
    {
        public string MemberName { get; set; } = null!;

        public int Rating { get; set; }

        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}