using Mapster;
using OfferManagement.Application.EntityServices.Companies.Models;
using OfferManagement.Application.Exceptions;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;
using System.ComponentModel.Design;

namespace OfferManagement.Application.EntityServices.Companies
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CompanyDTO> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            Company? company = await _unitOfWork.CompanyRepository.GetByIdAsync(id, cancellationToken);
            if (company == null) throw new NotFoundException($"Company by ID:{id} not found.");

            return company.Adapt<CompanyDTO>();
        }

        public async Task<CompanyDTO> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            Company? company = await _unitOfWork.CompanyRepository.GetByEmailAsync(email, cancellationToken);
            if (company == null) throw new NotFoundException($"Company by Email:{email} not found.");

            return company.Adapt<CompanyDTO>();
        }

        public async Task<IEnumerable<CompanyDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Company>? companies = await _unitOfWork.CompanyRepository.GetAllAsync(cancellationToken);
            if (!companies.Any()) return new List<CompanyDTO>();

            return companies.Adapt<IEnumerable<CompanyDTO>>();
        }

        public async Task CreateAsync(CreateCompanyDTO companyDto, CancellationToken cancellationToken)
        {
            await _unitOfWork.CompanyRepository.AddAsync(companyDto.Adapt<Company>(), cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task UpdateAsync(CompanyPutRequestModel companyPutRequestModel, CancellationToken cancellationToken)
        {
            Company? company = await _unitOfWork.CompanyRepository.GetByIdAsync(companyPutRequestModel.Id, cancellationToken);
            if (company == null) throw new NotFoundException($"Company by ID:{companyPutRequestModel.Id} not found.");

            var exists = await _unitOfWork.CompanyRepository.ExistsAsync(c => c.Email.Equals(companyPutRequestModel.Email), cancellationToken);
            if (exists) throw new AlreadyExistsException("Company with such email already exists");

            company.Name = companyPutRequestModel.Name;
            company.Email = companyPutRequestModel.Email;

            _unitOfWork.CompanyRepository.Update(company);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            Company? company = await _unitOfWork.CompanyRepository.GetByIdAsync(id, cancellationToken);
            if (company == null) throw new NotFoundException($"Company by ID:{id} not found.");

            _unitOfWork.CompanyRepository.Delete(company);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task ActivateAsync(int companyId,  CancellationToken cancellationToken)
        {
            Company? company = await _unitOfWork.CompanyRepository.GetByIdAsync(companyId, cancellationToken);
            if(company == null) throw new NotFoundException($"Company by ID:{companyId} not found.");

            company.IsActive = true;
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task DeactivateAsync(int companyId, CancellationToken cancellationToken)
        {
            Company? company = await _unitOfWork.CompanyRepository.GetByIdAsync(companyId, cancellationToken);
            if (company == null) throw new NotFoundException($"Company by ID:{companyId} not found.");

            company.IsActive = false;
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
