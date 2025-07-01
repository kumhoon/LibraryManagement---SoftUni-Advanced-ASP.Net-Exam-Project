namespace LibraryManagement.Data.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public Guid GenreId { get; set; }
        public virtual Author Author { get; set; } = null!;
        public virtual Genre Genre { get; set; } = null!;
        public DateTime PublishedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
