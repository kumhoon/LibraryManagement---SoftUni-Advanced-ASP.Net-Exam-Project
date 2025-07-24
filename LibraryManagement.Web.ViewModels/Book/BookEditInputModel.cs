namespace LibraryManagement.Web.ViewModels.Book
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.ComponentModel.DataAnnotations;
    using static LibraryManagement.Web.ViewModels.ViewModelValidationConstants.BookConstants;
    using static LibraryManagement.GCommon.ErrorMessages;
    public class BookEditInputModel
    {
        public Guid Id { get; set; }

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
