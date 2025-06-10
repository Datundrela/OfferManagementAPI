using OfferManagement.Application.Repositories;
using OfferManagement.Persistance.Context;

namespace OfferManagement.Infrastructure.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OfferManagementContext _context;

        public IUserRepository UserRepository { get; }
        public IOfferRepository OfferRepository { get; }
        public IPurchaseRepository PurchaseRepository { get; }
        public IAdminRepository AdminRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public ICompanyRepository CompanyRepository { get; }
        public ISubscriptionRepository SubscriptionRepository { get; }
        public IImageRepository ImageRepository { get; }

        public UnitOfWork(OfferManagementContext context,
                          IUserRepository userRepository,
                          IOfferRepository offerRepository,
                          IPurchaseRepository purchaseRepository,
                          IAdminRepository adminRepository,
                          ICategoryRepository categoryRepository,
                          ICompanyRepository companyRepository,
                          ISubscriptionRepository subscriptionRepository,
                          IImageRepository imageRepository)
        {
            _context = context;
            UserRepository = userRepository;
            OfferRepository = offerRepository;
            PurchaseRepository = purchaseRepository;
            AdminRepository = adminRepository;
            CategoryRepository = categoryRepository;
            CompanyRepository = companyRepository;
            SubscriptionRepository = subscriptionRepository;
            ImageRepository = imageRepository;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            await _context.Database.CommitTransactionAsync(cancellationToken);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            await _context.Database.RollbackTransactionAsync(cancellationToken);
        }

        public void Dispose() => _context.Dispose();
    }

}
