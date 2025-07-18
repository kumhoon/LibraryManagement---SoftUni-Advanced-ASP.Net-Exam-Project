﻿namespace LibraryManagement.Web.ViewModels.Review
{
    using System.ComponentModel.DataAnnotations;
    using static LibraryManagement.GCommon.ViewModelValidationConstants.ReviewConstants;
    using static LibraryManagement.GCommon.ViewModelValidationConstants.ErrorMessages;
    public class ReviewDisplayViewModel
    {
        [Required]
        public string MemberName { get; set; } = null!;
        [Required]
        [Range(ReviewRatingMinValue, ReviewRatingMaxValue, ErrorMessage = ReviewRatingErrorMessage)]
        public int Rating { get; set; }       
        [StringLength(ReviewContentMaxLength, MinimumLength = ReviewContentMinLength, ErrorMessage = ReviewContentErrorMessage)]
        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}