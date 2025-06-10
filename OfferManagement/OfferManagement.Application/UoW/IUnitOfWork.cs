using OfferManagement.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferManagement.Infrastructure.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IOfferRepository OfferRepository { get; }
        IPurchaseRepository PurchaseRepository { get; }
        IAdminRepository AdminRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        ISubscriptionRepository SubscriptionRepository { get; }
        IImageRepository ImageRepository { get; }

        Task CommitAsync(CancellationToken cancellationToken = default);

        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);
    }

}
