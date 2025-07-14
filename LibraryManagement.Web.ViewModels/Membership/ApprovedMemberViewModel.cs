namespace LibraryManagement.Web.ViewModels.Membership
{
    public class ApprovedMemberViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public DateTime JoinDate { get; set; }
    }
}
