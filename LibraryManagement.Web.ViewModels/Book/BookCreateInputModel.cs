namespace LibraryManagement.Web.ViewModels.Book
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using static LibraryManagement.GCommon.ViewModelValidationConstants.BookConstants;
    using static LibraryManagement.GCommon.ViewModelValidationConstants.ErrorMessages;
    public class BookCreateInputModel
    {
        [Required]
        [StringLength(BookTitleMaxLength, MinimumLength = BookTitleMinLength, ErrorMessage = BookTitleErrorMessage)]
        public string Title { get; set; } = null!;

        public Guid GenreId { get; set; }

        public IEnumerable<SelectListItem> Genres { get; set; } = Enumerable.Empty<SelectListItem>();

        [Required]
        [StringLength(BookDescriptionMaxLength, MinimumLength = BookDescriptionMinLength, ErrorMessage = BookDescriptionErrorMessage)]
        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }
        [Required]
        public string PublishedDate { get; set; } = null!;
        [Required]
        [StringLength(BookAuthorNameMaxLength, MinimumLength = BookAuthorNameMinLength, ErrorMessage = BookAuthorNameErrorMessage)]
        public string Author { get; set; } = null!;
    }
}
