using System.Linq.Expressions;

namespace LibraryManagement.Data.Interfaces
{
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
