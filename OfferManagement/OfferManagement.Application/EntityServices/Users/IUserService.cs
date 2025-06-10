using OfferManagement.Application.EntityServices.Users.Models;
using OfferManagement.Application.Users.Models;

namespace OfferManagement.Application.Users
{
    public interface IUserService
    {
        Task<UserDTO> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<UserDTO> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<IEnumerable<UserDTO>> GetAllAsync(CancellationToken cancellationToken);
        Task CreateAsync(CreateUserDTO userDto, CancellationToken cancellationToken);
        Task UpdateAsync(UserPutRequestModel userPutRequestModel, CancellationToken cancellationToken);
        Task DeleteByIdAsync(int id, CancellationToken cancellationToken);
        Task<TransactionResult> DecreaseBalanceAsync(int userId, decimal amount,  CancellationToken cancellationToken);
        Task<TransactionResult> IncreaseBalanceAsync(int userId, decimal amount, CancellationToken cancellationToken);
        Task<decimal> GetBalanceAsync(int userId, CancellationToken cancellationToken);
        Task IncreaseBalanceWithCommitAsync(int userId, decimal amount, CancellationToken cancellationToken);
        Task<TransactionResult> DepositAsync(int userId, decimal amount, CancellationToken cancellationToken);

    }
}
