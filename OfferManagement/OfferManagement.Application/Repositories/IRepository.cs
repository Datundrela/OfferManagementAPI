using OfferManagement.Domain.Entities;
using System.Linq.Expressions;

namespace OfferManagement.Application.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
        IQueryable<T> GetAllByPredicateQuery(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity, CancellationToken cancellationToken);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
