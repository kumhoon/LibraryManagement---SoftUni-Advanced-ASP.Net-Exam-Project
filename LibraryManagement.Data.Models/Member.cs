namespace LibraryManagement.Data.Models
{
    using Microsoft.AspNetCore.Identity;

    public class Member
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime JoinDate { get; set; }
        public virtual ICollection<BorrowingRecord> BorrowingRecords { get; set; } = new HashSet<BorrowingRecord>();

        public string UserId { get; set; } = null!;

        public virtual IdentityUser User { get; set; } = null!;

        public virtual MembershipStatus Status { get; set; } = MembershipStatus.None;

        public string? MembershipApplicationReason { get; set; }

        public virtual ICollection<FavouriteList> Favourites { get; set; } = new HashSet<FavouriteList>();

        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
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
