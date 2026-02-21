using AS.Fields.Domain.Entities;
using System.Linq.Expressions;

namespace AS.Fields.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        Task<T?> GetById(Guid id);
        IQueryable<T> QueryAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default);
    }
}
