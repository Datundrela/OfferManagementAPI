using OfferManagement.Application.Repositories;
using OfferManagement.Application.Users.Models;
using OfferManagement.Domain.Entities;
using OfferManagement.Application.Exceptions;
using Mapster;
using OfferManagement.Infrastructure.UoW;
using OfferManagement.Application.EntityServices.Users.Models;

namespace OfferManagement.Application.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDTO> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            User? user = await _unitOfWork.UserRepository.GetByIdAsync(id, cancellationToken);
            if(user == null) throw new NotFoundException($"User by ID:{id} not found.");

            return user.Adapt<UserDTO>();
        }

        public async Task<UserDTO> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            User? user = await _unitOfWork.UserRepository.GetByEmailAsync(email, cancellationToken);
            if (user == null) throw new NotFoundException($"User by Email:{email} not found.");

            return user.Adapt<UserDTO>();
        }

        public async Task<IEnumerable<UserDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            IEnumerable<User>? users = await _unitOfWork.UserRepository.GetAllAsync(cancellationToken);
            if(!users.Any()) return new List<UserDTO>();

            return users.Adapt<IEnumerable<UserDTO>>();
        }

        public async Task CreateAsync(CreateUserDTO userDto, CancellationToken cancellationToken)
        {
            await _unitOfWork.UserRepository.AddAsync(userDto.Adapt<User>(), cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task UpdateAsync(UserPutRequestModel userPutRequestModel, CancellationToken cancellationToken)
        {
            User? user = await _unitOfWork.UserRepository.GetByIdAsync(userPutRequestModel.Id, cancellationToken);
            if (user == null) throw new NotFoundException($"User by ID:{userPutRequestModel.Id} not found.");

            user.FirstName = userPutRequestModel.FirstName;
            user.LastName = userPutRequestModel.LastName;
            user.Email = userPutRequestModel.Email;

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            User? user = await _unitOfWork.UserRepository.GetByIdAsync(id, cancellationToken);
            if( user == null) throw new NotFoundException($"User by ID:{id} not found.");

            _unitOfWork.UserRepository.Delete(user);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task<TransactionResult> DecreaseBalanceAsync(int userId, decimal amount, CancellationToken cancellationToken)
        {
            User? user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null) throw new NotFoundException($"User by ID:{userId} not found.");

            if (user.Balance < amount) return new TransactionResult
            {
                Success = false,
                Message = "Not enough funds"
            };

            user.Balance -= amount;
            return new TransactionResult
            {
                Success = true,
                Message = "Payed successfully."
            };
        }

        public async Task<TransactionResult> IncreaseBalanceAsync(int userId, decimal amount, CancellationToken cancellationToken)
        {
            User? user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null) throw new NotFoundException($"User by ID:{userId} not found.");

            if(amount < 0)
            {
                return new TransactionResult
                {
                    Success = false,
                    Message = "Amount must be greater or equal to zero"
                };
            }

            user.Balance += amount;
            return new TransactionResult
            {
                Success = true,
                Message = "Increased successfully."
            };
        }

        public async Task<TransactionResult> DepositAsync(int userId, decimal amount, CancellationToken cancellationToken)
        {
            User? user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null) throw new NotFoundException($"User by ID:{userId} not found.");

            if (amount < 0)
            {
                return new TransactionResult
                {
                    Success = false,
                    Message = "Amount must be greater or equal to zero"
                };
            }

            user.Balance += amount;
            await _unitOfWork.CommitAsync(cancellationToken);
            return new TransactionResult
            {
                Success = true,
                Message = "Deposited successfully."
            };
        }

        public async Task<decimal> GetBalanceAsync(int userId, CancellationToken cancellationToken)
        {

            User? user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null) throw new NotFoundException($"User by ID:{userId} not found.");

            return user.Balance;
        }

        public async Task IncreaseBalanceWithCommitAsync(int userId, decimal amount, CancellationToken cancellationToken)
        {
            User? user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null) throw new NotFoundException($"User by ID:{userId} not found.");

            user.Balance += amount;
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
