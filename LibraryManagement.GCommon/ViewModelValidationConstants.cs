using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.GCommon
{
    public static class ViewModelValidationConstants
    {
        public static class BookConstants
        {
            public const int TitleMinLength = 1;
            public const int TitleMaxLength = 60;

            public const int BookAuthorNameMinLength = 1;
            public const int BookAuthorNameMaxLength = 50;

            public const int BookGenreNameMinLength = 1;
            public const int BookGenreNameMaxLength = 25;
        }

        public static class ErrorMessages
        {
            public const string BookTitleErrorMessage = "Title length must be between 1 and 60 characters";

            public const string BookAuthorNameErrorMessage = "Author name must be between 1 and 50 characters";

            public const string BookGenreNameErrorMessage = "Genre must be between 1 and 25 characters";
        }
    }
}
