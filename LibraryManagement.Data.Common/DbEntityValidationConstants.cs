using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
