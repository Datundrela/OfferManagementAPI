using OfferManagement.Application.EntityServices.Companies.Models;
using OfferManagement.Application.EntityServices.Users.Models;

namespace OfferManagement.Application.EntityServices.Companies
{
    public interface ICompanyService
    {
        Task<CompanyDTO> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<CompanyDTO> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<IEnumerable<CompanyDTO>> GetAllAsync(CancellationToken cancellationToken);
        Task CreateAsync(CreateCompanyDTO companyDto, CancellationToken cancellationToken);
        Task UpdateAsync(CompanyPutRequestModel companyPutRequestModel, CancellationToken cancellationToken);
        Task DeleteByIdAsync(int id, CancellationToken cancellationToken);
        Task ActivateAsync(int companyId, CancellationToken cancellationToken);
        Task DeactivateAsync(int companyId, CancellationToken cancellationToken);

    }
}
