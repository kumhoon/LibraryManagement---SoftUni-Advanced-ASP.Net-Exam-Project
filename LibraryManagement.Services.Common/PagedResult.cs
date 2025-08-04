namespace LibraryManagement.Services.Common
{
    /// <summary>
    /// Represents a paginated result set for service operations.
    /// </summary>
    /// <typeparam name="T">The type of items in the paginated result.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Gets the collection of items for the current page.
        /// </summary>
        public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();

        /// <summary>
        /// Gets the current page number (starting from 1).
        /// </summary>
        public int PageNumber { get; init; }

        /// <summary>
        /// Gets the number of items displayed per page.
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// Gets the total number of items across all pages.
        /// </summary>
        public int TotalItems { get; init; }

        /// <summary>
        /// Gets the total number of pages based on the total item count and page size.
        /// </summary>
        public int TotalPages
            => (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
