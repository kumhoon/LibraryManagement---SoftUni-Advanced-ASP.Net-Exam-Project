namespace LibraryManagement.Data.Common
{
    public static class DbEntityValidationConstants
    {
        public static class AuthorConstants
        {
            public const int AuthorNameMaxLength = 50;
        }

        public static class BookConstants
        {
            public const int BookTitleMaxLength = 60;
            public const int BookDescriptionMaxLength = 500;
            
        }

        public static class GenreConstants
        {
            public const int GenreNameMaxLength = 25;
        }

        public static class MemberConstants
        {
            public const int MemberNameMaxLength = 50;
        }

        public static class ReviewConstants
        {
            public const int ReviewContentMaxLength = 1000;
            public const int ReviewRatingMinValue = 1;
            public const int ReviewRatingMaxValue = 5;
        }
    }
}
