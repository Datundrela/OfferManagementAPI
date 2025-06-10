using Mapster;
using OfferManagement.Application.EntityServices.Admins.Models;
using OfferManagement.Application.EntityServices.Users.Models;
using OfferManagement.Application.Exceptions;
using OfferManagement.Domain.Entities;
using OfferManagement.Infrastructure.UoW;

namespace OfferManagement.Application.EntityServices.Admins
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminDTO> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            Administrator? admin = await _unitOfWork.AdminRepository.GetByIdAsync(id, cancellationToken);
            if (admin == null) throw new NotFoundException($"Admin by ID:{id} not found.");

            return admin.Adapt<AdminDTO>();
        }

        public async Task<AdminDTO> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            Administrator? admin = await _unitOfWork.AdminRepository.GetByEmailAsync(email, cancellationToken);
            if (admin == null) throw new NotFoundException($"Admin by Email:{email} not found.");

            return admin.Adapt<AdminDTO>();
        }

        public async Task<IEnumerable<AdminDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Administrator>? admins = await _unitOfWork.AdminRepository.GetAllAsync(cancellationToken);
            if (!admins.Any()) return new List<AdminDTO>();

            return admins.Adapt<IEnumerable<AdminDTO>>();
        }

        public async Task CreateAsync(CreateAdminDTO adminDTO, CancellationToken cancellationToken)
        {
            await _unitOfWork.AdminRepository.AddAsync(adminDTO.Adapt<Administrator>(), cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task UpdateAsync(UserPutRequestModel userPutRequestModel, CancellationToken cancellationToken)
        {
            Administrator? admin = await _unitOfWork.AdminRepository.GetByIdAsync(userPutRequestModel.Id, cancellationToken);
            if (admin == null) throw new NotFoundException($"Admin by ID:{userPutRequestModel.Id} not found.");

            admin.FirstName = userPutRequestModel.FirstName;
            admin.LastName = userPutRequestModel.LastName;
            admin.Email = userPutRequestModel.Email;

            _unitOfWork.AdminRepository.Update(admin);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            Administrator? admin = await _unitOfWork.AdminRepository.GetByIdAsync(id, cancellationToken);
            if (admin == null) throw new NotFoundException($"Admin by ID:{id} not found.");

            _unitOfWork.AdminRepository.Delete(admin);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
