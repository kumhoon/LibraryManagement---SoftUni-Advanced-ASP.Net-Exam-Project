namespace LibraryManagement.GCommon
{
    public static class Messages
    {
        public static class BorrowingRecordMessages
        {
            public const string BorrowSuccess = "You have successfully borrowed the book.";
            public const string ReturnSuccess = "You have successfully returned the book.";
            public const string BorrowLimitExceeded = "You can only borrow one book at a time. Please return your current book before borrowing another.";
            public const string BookAlreadyBorrowedByAnotherMember = "This book is currently borrowed by another user.";
            public const string BorrowFailed = "Failed to borrow the book. Please try again later.";
            public const string ReturnFailed = "Failed to return the book. Please try again later.";
            public const string BookAlreadyBorrowedByMember = "You have already borrowed this book.";
        }

        public static class FavouriteListMessages
        {
            public const string BookAddedToFavourites = "Book added to your favourites!";
            public const string BookAlreadyInFavourites = "This book is already in your favourites.";
            public const string BookRemovedFromFavourites = "Book removed from your favourites.";
            public const string BookNotFoundInFavourites = "This book was not found in your favourites.";
        }

        public static class ReviewMessages 
        {
            public const string ReviewAdded = "Your review was submitted successfully!";
        }

        public static class AdminMessages
        {
            public const string MembershipApproved = "Membership approved successfully.";
            public const string MembershipApprovedFailed = "Failed to approve membership.";

            public const string MembershipRejected = "Membership rejected successfully.";
            public const string MembershipRejectedFailed = "Failed to reject membership.";

            public const string MembershipRevoked = "Membership revoked successfully.";
            public const string MembershipRevokedFailed = "Failed to revoke membership.";

            public const string ReviewApproved = "Review approved successfully.";
            public const string ReviewRejected = "Review rejected successfully.";
        }
    }
}
