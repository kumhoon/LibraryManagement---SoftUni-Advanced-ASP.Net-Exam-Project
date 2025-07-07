namespace LibraryManagement.Web.ViewModels.Book
{
    public class BookDeleteInputModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;

        public string AuthorName { get; set; } = null!;
    }
}
