namespace LibraryManagement.Web.ViewModels.BorrowingRecord
{
    public class BorrowingRecordViewModel
    {
        public string Title { get; set; } = null!;
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
