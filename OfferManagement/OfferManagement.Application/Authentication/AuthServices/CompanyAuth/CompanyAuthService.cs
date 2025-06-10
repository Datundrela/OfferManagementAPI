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

namespace OfferManagement.Application.Authentication.AuthServices.CompanyAuth
{
    public class CompanyAuthService : ICompanyAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;


        public CompanyAuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthTokens> RegisterAsync(CompanyRegisterRequestModel request, CancellationToken cancellationToken)
        {
            var existingCompany = await _unitOfWork.CompanyRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingCompany != null) throw new AlreadyExistsException($"Company by email: {request.Email} already exists");

            byte[] passwordHash, passwordSalt;
            _passwordHasher.HashPassword(request.Password, out passwordHash, out passwordSalt);

            var company = new Company
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = Role.Company
            };

            AuthTokens authTokens = _tokenService.GenerateTokens(company);
            company.RefreshToken = authTokens.RefreshToken;
            company.RefreshTokenExpiration = _tokenService.GetRefreshTokenExpriationTIme();

            await _unitOfWork.CompanyRepository.AddAsync(company, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return authTokens;
        }

        public async Task<LoginResponseModel> LoginAsync(LoginRequestModel request, CancellationToken cancellationToken)
        {
            var company = await _unitOfWork.CompanyRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (company == null || !_passwordHasher.VerifyPassword(request.Password, company.PasswordHash, company.PasswordSalt))
                return new LoginResponseModel
                {
                    Success = false,
                    Message = "Incorrect email or password"
                };

            AuthTokens authTokens = _tokenService.GenerateTokens(company);

            company.RefreshToken = authTokens.RefreshToken;
            company.RefreshTokenExpiration = _tokenService.GetRefreshTokenExpriationTIme();
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

            var company = await _unitOfWork.CompanyRepository.GetByEmailAsync(email, cancellationToken);

            if (company == null || company.RefreshToken != tokens.RefreshToken || company.RefreshTokenExpiration <= DateTime.UtcNow)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            AuthTokens newTokens = _tokenService.GenerateTokens(company);

            company.RefreshToken = newTokens.RefreshToken;
            company.RefreshTokenExpiration = _tokenService.GetRefreshTokenExpriationTIme();
            await _unitOfWork.CommitAsync(cancellationToken);

            return newTokens;
        }
    }
}
