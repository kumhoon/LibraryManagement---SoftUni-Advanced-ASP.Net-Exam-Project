﻿namespace LibraryManagement.GCommon
{
    public static class ViewModelValidationConstants
    {
        public static class BookConstants
        {
            public const int BookTitleMinLength = 1;
            public const int BookTitleMaxLength = 60;

            public const int BookAuthorNameMinLength = 1;
            public const int BookAuthorNameMaxLength = 50;

            public const int BookGenreNameMinLength = 1;
            public const int BookGenreNameMaxLength = 25;

            public const int BookDescriptionMinLength = 1;
            public const int BookDescriptionMaxLength = 500;

            public const string PublishedOnDateTimeFormat = "dd-MM-yyyy";
        }

        public static class ErrorMessages
        {
            public const string BookTitleErrorMessage = "Title length must be between 1 and 60 characters";

            public const string BookAuthorNameErrorMessage = "Author name must be between 1 and 50 characters";

            public const string BookGenreNameErrorMessage = "Genre must be between 1 and 25 characters";

            public const string BookDescriptionErrorMessage = "Description must be between 1 and 500 characters";

            public const string MemberNameErrorMessage = "Name must be between 1 and 60 characters";

            public const string ReviewContentErrorMessage = "Review content must be between 1 and 1000 characters";

            public const string ReviewRatingErrorMessage = "Rating must be between 1 and 5";
        }

        public static class MembershipConstants
        {
            public const int MemberNameMinLength = 1;
            public const int MemberNameMaxLength = 60;
        }

        public static class ReviewConstants
        {
            public const int ReviewContentMinLength = 1;
            public const int ReviewContentMaxLength = 1000;
            public const int ReviewRatingMinValue = 1;
            public const int ReviewRatingMaxValue = 5;
            public const string ReviewDateTimeFormat = "dd-MM-yyyy";
        }
    }
}
