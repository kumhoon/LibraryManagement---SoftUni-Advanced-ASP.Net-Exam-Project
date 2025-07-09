using LibraryManagement.Data.Models;

namespace LibraryManagement.Web.ViewModels.Membership
{
    public class MembershipPendingViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateTime JoinDate { get; set; }

        public string? Reason { get; set; }

        public MembershipStatus Status { get; set; }
    }
}
