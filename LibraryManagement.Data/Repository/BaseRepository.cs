namespace LibraryManagement.Data.Repository
{
    using LibraryManagement.Data.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Linq.Expressions;
    using static LibraryManagement.GCommon.ErrorMessages;

    /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<TType?> GetByIdAsync(TId? id)
        {
            TType? entity = await this.dbSet
                .FindAsync(id);

            return entity;
        }

        /// <inheritdoc />
        public async Task<TType?> FirstOrDefaultAsync(Expression<Func<TType, bool>> predicate)
        {
            TType? entity = await this.dbSet
                .FirstOrDefaultAsync(predicate);

            return entity;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TType>> GetAllAsync()
        {
            return await this.dbSet.ToArrayAsync();
        }

        /// <inheritdoc />
        public IQueryable<TType> GetAllAttached()
        {
            return this.dbSet.AsQueryable();
        }

        /// <inheritdoc />
        public async Task AddAsync(TType item)
        {
            await this.dbSet.AddAsync(item);
            await this.dbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task AddRangeAsync(TType[] items)
        {
            await this.dbSet.AddRangeAsync(items);
            await this.dbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task SaveChangesAsync()
        {
            await this.dbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
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
                throw new InvalidOperationException(MissingIsDeletedPropertyErrorMessage);
            }
        }
    }
     
}