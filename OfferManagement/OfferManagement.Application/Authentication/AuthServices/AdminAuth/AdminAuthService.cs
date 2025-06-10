using OfferManagement.Application.Authentication.AuthServices.Models;
using OfferManagement.Application.Authentication.TokenServices;
using OfferManagement.Application.AuthServices.JWT.Models;
using OfferManagement.Domain.Entities;
using OfferManagement.Domain;
using OfferManagement.Infrastructure.Security.PasswordHashing;
using OfferManagement.Infrastructure.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using OfferManagement.Application.Exceptions;

namespace OfferManagement.Application.Authentication.AuthServices.AdminAuth
{
    public class AdminAuthService : IAdminAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;


        public AdminAuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthTokens> RegisterAsync(UserRegisterRequestModel request, CancellationToken cancellationToken)
        {
            var existingUser = await _unitOfWork.AdminRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null) throw new AlreadyExistsException($"Admin by email: {request.Email} already exists");

            byte[] passwordHash, passwordSalt;
            _passwordHasher.HashPassword(request.Password, out passwordHash, out passwordSalt);

            var admin = new Administrator
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = Role.Admin
            };

            AuthTokens authTokens = _tokenService.GenerateTokens(admin);
            admin.RefreshToken = authTokens.RefreshToken;
            admin.RefreshTokenExpiration = _tokenService.GetRefreshTokenExpriationTIme();

            await _unitOfWork.AdminRepository.AddAsync(admin, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return authTokens;
        }

        public async Task<LoginResponseModel> LoginAsync(LoginRequestModel request, CancellationToken cancellationToken)
        {
            var admin = await _unitOfWork.AdminRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (admin == null || !_passwordHasher.VerifyPassword(request.Password, admin.PasswordHash, admin.PasswordSalt))
                return new LoginResponseModel
                {
                    Success = false,
                    Message = "Incorrect email or password"
                };

            AuthTokens authTokens = _tokenService.GenerateTokens(admin);

            admin.RefreshToken = authTokens.RefreshToken;
            admin.RefreshTokenExpiration = _tokenService.GetRefreshTokenExpriationTIme();
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

            var admin = await _unitOfWork.AdminRepository.GetByEmailAsync(email, cancellationToken);

            if (admin == null || admin.RefreshToken != tokens.RefreshToken || admin.RefreshTokenExpiration <= DateTime.UtcNow)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            AuthTokens newTokens = _tokenService.GenerateTokens(admin);

            admin.RefreshToken = newTokens.RefreshToken;
            admin.RefreshTokenExpiration = _tokenService.GetRefreshTokenExpriationTIme();
            await _unitOfWork.CommitAsync(cancellationToken);

            return newTokens;
        }
    }
}
