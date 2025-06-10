using OfferManagement.Application.EntityServices.Admins.Models;
using OfferManagement.Application.EntityServices.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.EntityServices.Admins
{
    public interface IAdminService
    {
        Task<AdminDTO> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<AdminDTO> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<IEnumerable<AdminDTO>> GetAllAsync(CancellationToken cancellationToken);
        Task CreateAsync(CreateAdminDTO adminDTO, CancellationToken cancellationToken);
        Task UpdateAsync(UserPutRequestModel userPutRequestModel, CancellationToken cancellationToken);
        Task DeleteByIdAsync(int id, CancellationToken cancellationToken);
    }
}
