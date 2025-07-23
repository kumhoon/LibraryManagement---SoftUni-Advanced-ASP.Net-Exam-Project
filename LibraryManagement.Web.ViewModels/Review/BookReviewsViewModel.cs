namespace LibraryManagement.Web.ViewModels.Review
{
    using LibraryManagement.Services.Common;

    public class BookReviewsViewModel
    {
        public Guid BookId { get; set; }
        public double AverageRating { get; set; }        
        public int TotalReviews { get; set; }          
        public PagedResult<ReviewDisplayInputModel> Reviews { get; set; } = new ();
    }
}
