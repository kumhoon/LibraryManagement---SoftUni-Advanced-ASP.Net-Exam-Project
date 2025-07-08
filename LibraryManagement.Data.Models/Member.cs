using Microsoft.AspNetCore.Identity;

namespace LibraryManagement.Data.Models
{
    public class Member
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime JoinDate { get; set; }
        public virtual ICollection<BorrowingRecord> BorrowingRecords { get; set; } = new HashSet<BorrowingRecord>();

        public string UserId { get; set; } = null!;

        public IdentityUser User { get; set; } = null!;

        public MembershipStatus Status { get; set; } = MembershipStatus.None;

        public string? MembershipApplicationReason { get; set; }

    }
    public enum MembershipStatus
    {
        None,
        Pending,
        Approved,
        Rejected,
        Revoked
    }
}
