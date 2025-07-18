namespace LibraryManagement.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static LibraryManagement.Data.Common.DbEntityValidationConstants.ReviewConstants;
    public class Review
    {
        public Guid Id { get; set; }

        public Guid MemberId { get; set; }
        public Member Member { get; set; } = null!;

        public Guid BookId { get; set; }
        public Book Book { get; set; } = null!;

        [Range(ReviewRatingMinValue, ReviewRatingMaxValue)]
        public int Rating { get; set; } 

        public string? Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsApproved { get; set; }
    }
}
