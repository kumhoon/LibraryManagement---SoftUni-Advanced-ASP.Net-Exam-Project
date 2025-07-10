namespace LibraryManagement.Web.ViewModels.BorrowingRecord
{
    public class BorrowingRecordResultViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public string BookTitle { get; set; } = null!;
    }
}
