using LibraryManagement.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly LibraryManagementDbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(LibraryManagementDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToArrayAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity == null)
                return;

            var isDeletedProp = typeof(T).GetProperty("IsDeleted");
            if (isDeletedProp != null && isDeletedProp.PropertyType == typeof(bool))
            {
                isDeletedProp.SetValue(entity, true);
                _dbSet.Update(entity);
            }
            else
            {
                throw new InvalidOperationException($"{typeof(T).Name} does not support soft deletion (missing IsDeleted property).");
            }
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
