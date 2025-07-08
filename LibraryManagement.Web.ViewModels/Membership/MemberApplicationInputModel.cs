namespace LibraryManagement.Web.ViewModels.Membership
{
    using System.ComponentModel.DataAnnotations;
    using static LibraryManagement.GCommon.ViewModelValidationConstants.MembershipConstants;
    using static LibraryManagement.GCommon.ViewModelValidationConstants.ErrorMessages;
    public class MemberApplicationInputModel
    {
        [Required]
        [StringLength(MemberNameMaxLength, MinimumLength = MemberNameMinLength, ErrorMessage = MemberNameErrorMessage)]
        public string Name { get; set; } = null!;

        public DateTime JoinDate { get; set; }

        public string? Reason { get; set; }
    }
}
