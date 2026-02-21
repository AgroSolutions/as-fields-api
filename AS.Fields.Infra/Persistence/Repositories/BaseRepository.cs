using AS.Fields.Domain.Entities;
using AS.Fields.Domain.Interfaces.Repositories;
using AS.Fields.Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AS.Fields.Infra.Persistence.Repositories
{
    public abstract class BaseRepository<TEntity>(ASFieldsContext context) : IBaseRepository<TEntity>
        where TEntity : BaseEntity
    {
        protected readonly DbContext _context = context;
        protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

        public virtual IEnumerable<TEntity> GetAll()
            => _dbSet.AsNoTracking().AsEnumerable();

        public Task<TEntity?> GetById(Guid id) => _dbSet.FirstOrDefaultAsync(x => x.Id == id);

        public virtual IQueryable<TEntity> QueryAsync(Expression<Func<TEntity, bool>> predicate)
            => _dbSet.Where(predicate);

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }


    }
}
