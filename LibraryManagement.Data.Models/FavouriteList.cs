namespace LibraryManagement.Data.Models
{
    public class FavouriteList
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public virtual Member Member { get; set; } = null!;

        public Guid BookId { get; set; }
        public virtual Book Book { get; set; } = null!;

        public DateTime AddedAt { get; set; }
    }
}
