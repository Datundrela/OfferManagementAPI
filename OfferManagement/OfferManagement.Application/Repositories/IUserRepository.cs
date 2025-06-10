using OfferManagement.Domain.Entities;
using System.Linq.Expressions;

namespace OfferManagement.Application.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
