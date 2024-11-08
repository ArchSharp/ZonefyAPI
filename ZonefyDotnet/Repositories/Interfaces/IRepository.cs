using System.Linq.Expressions;

namespace ZonefyDotnet.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(Guid id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAllPaginatedAsync(int skip, int take, Expression<Func<TEntity, object>> orderBy);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> FindPaginatedAsync(Expression<Func<TEntity, bool>> predicate, int skip, int take, Expression<Func<TEntity, object>> orderBy);
        bool Exists(Expression<Func<TEntity, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entity);
        void Remove(TEntity entity);
        Task<bool> SaveChangesAsync();
        void Update(TEntity entity);
        Task<TEntity> SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> SingleOrDefaultNoTracking(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultNoTracking(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        void UpdateRange(IEnumerable<TEntity> entity);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> QueryableEntity(Expression<Func<TEntity, bool>> predicate = null);
    }
}
