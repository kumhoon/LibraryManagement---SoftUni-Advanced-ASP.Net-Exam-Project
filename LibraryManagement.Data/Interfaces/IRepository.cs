namespace LibraryManagement.Data.Interfaces
{
    using System.Linq.Expressions;
    public interface IRepository<TType, TId>
    {
        Task<TType> GetByIdAsync(TId id);

        Task<TType> FirstOrDefaultAsync(Expression<Func<TType, bool>> predicate);

        Task<IEnumerable<TType>> GetAllAsync();

        IQueryable<TType> GetAllAttached();

        Task AddAsync(TType item);

        Task AddRangeAsync(TType[] items);

        Task<bool> UpdateAsync(TType item);

        Task SaveChangesAsync();

        Task SoftDeleteAsync(Guid id);
    }
       
}
