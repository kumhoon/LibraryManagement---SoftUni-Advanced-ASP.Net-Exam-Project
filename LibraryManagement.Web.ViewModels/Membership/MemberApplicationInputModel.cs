namespace LibraryManagement.Web.ViewModels.Membership
{
    using System.ComponentModel.DataAnnotations;
    using static LibraryManagement.Web.ViewModels.ViewModelValidationConstants.MembershipConstants;
    using static LibraryManagement.GCommon.ErrorMessages;
    public class MemberApplicationInputModel
    {
        [Required]
        [StringLength(MemberNameMaxLength, MinimumLength = MemberNameMinLength, ErrorMessage = MemberNameErrorMessage)]
        public string Name { get; set; } = null!;

        public DateTime JoinDate { get; set; }

        public string? Reason { get; set; }
    }
}
