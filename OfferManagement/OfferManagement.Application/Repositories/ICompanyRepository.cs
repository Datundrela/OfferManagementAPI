using OfferManagement.Domain.Entities;

namespace OfferManagement.Application.Repositories
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<Company?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<bool> IsActiveAsync(int companyId, CancellationToken cancellationToken);
    }
}
