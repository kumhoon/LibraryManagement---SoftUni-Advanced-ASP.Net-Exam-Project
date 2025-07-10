namespace LibraryManagement.Data.Models
{
    public class BorrowingRecord
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public Guid MemberId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public virtual Book? Book { get; set; }
        public virtual Member Member { get; set; } = null!;
    }
}
