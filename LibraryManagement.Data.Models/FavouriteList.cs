namespace LibraryManagement.Data.Models
{
    public class FavouriteList
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public Member Member { get; set; } = null!;

        public Guid BookId { get; set; }
        public Book Book { get; set; } = null!;

        public DateTime AddedAt { get; set; }
    }
}
