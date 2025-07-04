namespace LibraryManagement.Data.Models
{
    public class Author
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<Book> Books { get; set; } = new HashSet<Book>();

        public bool IsDeleted { get; set; }

        public string? ImageUrl { get; set; }
    }
}
