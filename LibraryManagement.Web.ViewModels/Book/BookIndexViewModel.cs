namespace LibraryManagement.Web.ViewModels.Book
{
    using System.ComponentModel.DataAnnotations;
    using static LibraryManagement.GCommon.ViewModelValidationConstants.BookConstants;
    using static LibraryManagement.GCommon.ViewModelValidationConstants.ErrorMessages;
    public class BookIndexViewModel
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength, ErrorMessage = BookTitleErrorMessage)]
        public string Title { get; set; } = null!;
        [Required]
        [StringLength(BookAuthorNameMaxLength, MinimumLength = BookAuthorNameMinLength, ErrorMessage = BookAuthorNameErrorMessage)]
        public string AuthorName { get; set; } = null!;
        [Required]
        [StringLength(BookGenreNameMaxLength, MinimumLength = BookGenreNameMinLength, ErrorMessage = BookGenreNameErrorMessage)]
        public string Genre { get; set; } = null!;
        public DateTime PublishedDate { get; set; }
    }
}
