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

        public const string NegativePageNumberErrorMessage = "Page number must be greater than zero.";

        public const string NegativePageSizeErrorMessage = "Page size must be greater than zero.";

        public const string InvalidPaginationValues = "Invalid pagination values provided.";

        public const string LoadingAuthorsErrorMessage = "An error occurred while loading authors.";

        public const string InvalidGenreErrorMessage = "Invalid genre selected";

        public const string InvalidBookErrorMessage = "Invalid book selected";

        public const string BookNotFoundErrorMessage = "Book not found.";

        public const string NotAuthorizedErrorMessage = "You are not authorized to do this action.";

        public const string GenreNotFoundErrorMessage = "Genre not found.";

        public const string InvalidPublishedDateFormatErrorMessage = "Invalid published date format.";

        public const string BorrowingRecordNotFoundErrorMessage = "No active borrowing record found for this book and member.";

        public const string BorrowingRecordUpdateErorrMessage = "Failed to update the borrowing record.";

        public const string ApproveMembershipErrorMessage = "Error approving membership.";

        public const string RejectMembershipErrorMessage = "Error rejecting membership.";

        public const string RevokeMembershipErrorMessage = "Error revoking membership.";

        public const string UnexpectedErrorMessage = "An unexpected error occurred. Please try again later.";

        public const string PendingReviewsLoadingErrorMessage = "Failed to load pending reviews.";

        public const string ReviewApproveErrorMessage = "Failed to approve the review. Please try again.";

        public const string ReviewRejectErrorMessage = "Failed to reject the review. Please try again.";

        public const string BookEditErrorMessage = "Error loading book for editing.";

        public const string BookUpdateErrorMessage = "Error updating book.";

        public const string InvalidDataErrorMessage = "Invalid data provided. Please try again.";

        public const string BookDeleteErrorMessage = "Error deleting book.";

        public const string BorrowingHistoryErrorMessage = "An error occurred while loading borrowing history.";

        public const string FavouriteListErrorMessage = "An error occurred while loading the favourite list.";

        public const string FavouriteListAddErrorMessage = "Error adding book to favourites.";

        public const string FavouriteListRemoveErrorMessage = "Error removing book from favourites.";

        public const string MembershipApplicationPageErrorMessage = "Error loading membership application page.";

        public const string MembershipApplicationErrorMessage = "Error applying for membership.";

        public const string ReviewSubmitErrorMessage = "An error occurred while submitting your review.";

        public const string UserDashboardErrorMessage = "Error loading user dashboard";
    }
}
