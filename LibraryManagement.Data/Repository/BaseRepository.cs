using LibraryManagement.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibraryManagement.Data.Repository
{
    public class BaseRepository<TType, TId> : IRepository<TType, TId>
        where TType : class
    {
        private readonly LibraryManagementDbContext dbContext;
        private readonly DbSet<TType> dbSet;

        public BaseRepository(LibraryManagementDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = this.dbContext.Set<TType>();
        }
      
        public async Task<TType> GetByIdAsync(TId? id)
        {
            TType? entity = await this.dbSet
                .FindAsync(id);

            return entity;
        }

        public async Task<TType> FirstOrDefaultAsync(Expression<Func<TType, bool>> predicate)
        {
            TType? entity = await this.dbSet
                .FirstOrDefaultAsync(predicate);

            return entity;
        }     

        public async Task<IEnumerable<TType>> GetAllAsync()
        {
            return await this.dbSet.ToArrayAsync();
        }

        public IQueryable<TType> GetAllAttached()
        {
            return this.dbSet.AsQueryable();
        }
        public async Task AddAsync(TType item)
        {
            await this.dbSet.AddAsync(item);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task AddRangeAsync(TType[] items)
        {
            await this.dbSet.AddRangeAsync(items);
            await this.dbContext.SaveChangesAsync();
        }   

        public async Task<bool> UpdateAsync(TType item)
        {
            try
            {
                this.dbSet.Attach(item);
                this.dbContext.Entry(item).State = EntityState.Modified;
                await this.dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }   

        public async Task SaveChangesAsync()
        {
            await this.dbContext.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var entity = await dbSet.FindAsync(id);

            if (entity == null)
                return;

            var isDeletedProp = typeof(TType).GetProperty("IsDeleted");
            if (isDeletedProp != null && isDeletedProp.PropertyType == typeof(bool))
            {
                isDeletedProp.SetValue(entity, true);
                dbSet.Update(entity);
            }
            else
            {
                throw new InvalidOperationException($"{typeof(TType).Name} does not support soft deletion (missing IsDeleted property).");
            }
        }
    }
     
}