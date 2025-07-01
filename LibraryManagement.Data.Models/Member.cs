namespace LibraryManagement.Data.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime JoinDate { get; set; }
        public virtual ICollection<BorrowingRecord> BorrowingRecords { get; set; } = new HashSet<BorrowingRecord>();
    }
}
