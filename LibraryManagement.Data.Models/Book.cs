namespace LibraryManagement.Data.Models
{
    using Microsoft.AspNetCore.Identity;
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
        public string BookCreatorId { get; set; } = null!;
        public virtual IdentityUser BookCreator { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public virtual ICollection<FavouriteList> FavouritedBy { get; set; } = new HashSet<FavouriteList>();

        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    }
}
