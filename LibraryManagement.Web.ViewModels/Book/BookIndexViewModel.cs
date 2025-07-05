namespace LibraryManagement.Web.ViewModels.Book
{
    using System.ComponentModel.DataAnnotations;
    using static LibraryManagement.GCommon.ViewModelValidationConstants.BookConstants;
    using static LibraryManagement.GCommon.ViewModelValidationConstants.ErrorMessages;
    public class BookIndexViewModel
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; } = null!;
        
        public string AuthorName { get; set; } = null!;
       
        public string Genre { get; set; } = null!;
        public DateTime PublishedDate { get; set; }

        public string? ImageUrl { get; set; }
    }
}
