using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Data.Common
{
    public static class DbEntityValidationConstants
    {
        public static class Author
        {
            public const int AuthorNameMaxLength = 50;
        }

        public static class Book
        {
            public const int BookTitleMaxLength = 60;
            
        }

        public static class Genre
        {
            public const int GenreNameMaxLength = 25;
        }
    }
}
