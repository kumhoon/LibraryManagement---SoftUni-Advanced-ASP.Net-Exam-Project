﻿namespace LibraryManagement.Web.ViewModels.Review
{
    using System.ComponentModel.DataAnnotations;
    using static LibraryManagement.Web.ViewModels.ViewModelValidationConstants.ReviewConstants;
    using static LibraryManagement.GCommon.ErrorMessages;

    public class ReviewInputModel
    {
        public Guid ReviewId { get; set; }

        public Guid BookId { get; set; }
        [Required]
        [Range(ReviewRatingMinValue, ReviewRatingMaxValue, ErrorMessage = ReviewRatingErrorMessage)]
        public int Rating { get; set; }
        [StringLength(ReviewContentMaxLength, MinimumLength = ReviewContentMinLength, ErrorMessage = ReviewContentErrorMessage)]
        public string? Content { get; set; }

    }
}
