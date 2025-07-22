namespace LibraryManagement.GCommon
{
    public static class ErrorMessages
    {
        public const string BookTitleErrorMessage = "Title length must be between 1 and 60 characters";

        public const string BookAuthorNameErrorMessage = "Author name must be between 1 and 50 characters";

        public const string BookGenreNameErrorMessage = "Genre must be between 1 and 25 characters";

        public const string BookDescriptionErrorMessage = "Description must be between 1 and 500 characters";

        public const string MemberNameErrorMessage = "Name must be between 1 and 60 characters";

        public const string ReviewContentErrorMessage = "Review content must be between 1 and 1000 characters";

        public const string ReviewRatingErrorMessage = "Rating must be between 1 and 5";

        public const string MissingIsDeletedPropertyErrorMessage = "The entity must have an IsDeleted property of type bool to support soft deletion.";

        public const string UserNotFoundErrorMessage = "User not found";

        public const string MembershipApprovedOrPendingErrorMessage = "You have already applied for or have been granted membership.";
    }
}
