namespace LibraryManagement.GCommon
{
    using static ErrorMessages;
    public static class PaginationValidator
    {
        public static void Validate(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), NegativePageNumberErrorMessage);

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), NegativePageSizeErrorMessage);
        }
    }
}
