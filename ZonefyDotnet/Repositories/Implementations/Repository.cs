using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ZonefyDotnet.Database;
using ZonefyDotnet.Repositories.Interfaces;


namespace ZonefyDotnet.Repositories.Implementations
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            await _context.Set<TEntity>().AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            await _context.Set<TEntity>().AddRangeAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().Any(predicate);
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().AnyAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().CountAsync(predicate);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);

            if (entity == null) return null;

            return entity;
        }

        public void Remove(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _context.Set<TEntity>().Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(TEntity entity)
        {

        }
        public Task<TEntity> SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().SingleOrDefaultAsync(predicate)!;
        }

#pragma warning disable CS8619
        public Task<TEntity> SingleOrDefaultNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(predicate);
        }
#pragma warning restore CS8619

#pragma warning disable CS8619
        public Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }
#pragma warning restore CS8619

#pragma warning disable CS8619
        public Task<TEntity> FirstOrDefaultNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(predicate);
        }
#pragma warning restore CS8619
        public void UpdateRange(IEnumerable<TEntity> entity)
        {
            _context.Set<TEntity>().UpdateRange(entity);
        }

        public IQueryable<TEntity> QueryableEntity(Expression<Func<TEntity, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return _context.Set<TEntity>().AsQueryable();
            }
            return _context.Set<TEntity>().AsQueryable()
                .Where(predicate);
        }

        public async Task<IEnumerable<TEntity>> GetAllPaginatedAsync(int skip, int take, Expression<Func<TEntity, object>> orderBy)
        {
            return await _context.Set<TEntity>()
                .OrderBy(orderBy)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindPaginatedAsync(Expression<Func<TEntity, bool>> predicate, int skip, int take, Expression<Func<TEntity, object>> orderBy)
        {
            return await _context.Set<TEntity>()
                        .Where(predicate)
                        .OrderBy(orderBy)
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync();
        }
    }
}
