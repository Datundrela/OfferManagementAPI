using OfferManagement.Domain.Entities;

namespace OfferManagement.Application.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken);
    }
}
