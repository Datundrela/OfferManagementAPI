using OfferManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Application.Repositories
{
    public interface IAdminRepository : IRepository<Administrator>
    {
        Task<Administrator?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
