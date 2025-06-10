using OfferManagement.Application.Authentication.AuthServices.Models;
using OfferManagement.Application.Authentication.TokenServices;
using OfferManagement.Application.AuthServices.JWT.Models;
using OfferManagement.Domain.Entities;
using OfferManagement.Domain;
using OfferManagement.Infrastructure.UoW;
using OfferManagement.Infrastructure.Security.PasswordHashing;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using OfferManagement.Application.Exceptions;

namespace OfferManagement.Application.Authentication.AuthServices.UserAuth
{
    public class UserAuthService : IUserAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;


        public UserAuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthTokens> RegisterAsync(UserRegisterRequestModel request, CancellationToken cancellationToken)
        {
            var existingUser = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null) throw new AlreadyExistsException($"User by email: {request.Email} already exists");

            byte[] passwordHash, passwordSalt;
            _passwordHasher.HashPassword(request.Password, out passwordHash, out passwordSalt);

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = Role.User
            };

            AuthTokens authTokens = _tokenService.GenerateTokens(user);
            user.RefreshToken = authTokens.RefreshToken;
            user.RefreshTokenExpiration = _tokenService.GetRefreshTokenExpriationTIme();

            await _unitOfWork.UserRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return authTokens;
        }

        public async Task<LoginResponseModel> LoginAsync(LoginRequestModel request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
                return new LoginResponseModel
                {
                    Success = false,
                    Message = "Incorrect email or password"
                };

            AuthTokens authTokens = _tokenService.GenerateTokens(user);

            user.RefreshToken = authTokens.RefreshToken;
            user.RefreshTokenExpiration = _tokenService.GetRefreshTokenExpriationTIme();
            await _unitOfWork.CommitAsync(cancellationToken);

            return new LoginResponseModel
            {
                Success = true,
                Tokens = authTokens
            };
        }

        public async Task<AuthTokens> RotateTokensAsync(AuthTokens tokens, CancellationToken cancellationToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(tokens.AccessToken);
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;

            if (email == null) throw new SecurityTokenException("Invalid token");

            var user = await _unitOfWork.UserRepository.GetByEmailAsync(email, cancellationToken);

            if (user == null || user.RefreshToken != tokens.RefreshToken || user.RefreshTokenExpiration <= DateTime.UtcNow)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            AuthTokens newTokens = _tokenService.GenerateTokens(user);

            user.RefreshToken = newTokens.RefreshToken;
            user.RefreshTokenExpiration = _tokenService.GetRefreshTokenExpriationTIme();
            await _unitOfWork.CommitAsync(cancellationToken);

            return newTokens;
        }
    }
}
