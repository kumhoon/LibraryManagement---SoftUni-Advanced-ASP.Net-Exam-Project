namespace LibraryManagement.Data.Interfaces
{
    using System.Linq.Expressions;

    /// <summary>
    /// Generic repository interface defining common data access operations.
    /// </summary>
    /// <typeparam name="TType">The entity type.</typeparam>
    /// <typeparam name="TId">The type of the entity's identifier.</typeparam>
    public interface IRepository<TType, TId>
    {
        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>
        /// The entity if found; otherwise, <c>null</c>.
        /// </returns>
        Task<TType?> GetByIdAsync(TId? id);

        /// <summary>
        /// Retrieves the first entity matching the specified predicate or <c>null</c> if none found.
        /// </summary>
        /// <param name="predicate">A filter expression to match entities.</param>
        /// <returns>
        /// The first matching entity or <c>null</c>.
        /// </returns>
        Task<TType?> FirstOrDefaultAsync(Expression<Func<TType, bool>> predicate);

        /// <summary>
        /// Retrieves all entities of type <typeparamref name="TType"/>.
        /// </summary>
        /// <returns>
        /// A collection of all entities.
        /// </returns>
        Task<IEnumerable<TType>> GetAllAsync();

        /// <summary>
        /// Retrieves all entities of type <typeparamref name="TType"/> with tracking enabled.
        /// </summary>
        /// <returns>
        /// An <see cref="IQueryable{TType}"/> of all tracked entities.
        /// </returns>
        IQueryable<TType> GetAllAttached();

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        Task AddAsync(TType item);

        /// <summary>
        /// Adds multiple new entities to the repository.
        /// </summary>
        /// <param name="items">The entities to add.</param>
        Task AddRangeAsync(TType[] items);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="item">The entity to update.</param>
        /// <returns>
        /// <c>true</c> if the update was successful; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> UpdateAsync(TType item);

        /// <summary>
        /// Saves all changes made in the repository to the database.
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// Performs a soft delete on an entity identified by its ID.
        /// </summary>
        /// <param name="id">The identifier of the entity to soft delete.</param>
        Task SoftDeleteAsync(Guid id);
    }
       
}
