using OfferManagement.Application.EntityServices.Admins;
using OfferManagement.Application.EntityServices.Categories;
using OfferManagement.Application.EntityServices.Companies;
using OfferManagement.Application.EntityServices.Offers;
using OfferManagement.Application.EntityServices.Purchases;
using OfferManagement.Application.EntityServices.Subscriptions;
using OfferManagement.Application.Purchases;
using OfferManagement.Application.Repositories;
using OfferManagement.Application.Users;
using OfferManagement.Infrastructure.Admins;
using OfferManagement.Infrastructure.Categories;
using OfferManagement.Infrastructure.Companies;
using OfferManagement.Infrastructure.Offers;
using OfferManagement.Infrastructure.Purchases;
using OfferManagement.Infrastructure.Subscriptions;
using OfferManagement.Infrastructure.UoW;
using OfferManagement.Infrastructure.Users;
using OfferManagement.Infrastructure.Security.PasswordHashing;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OfferManagement.Application.AuthServices.JWT;
using OfferManagement.Application.Authentication.TokenServices;
using OfferManagement.Application.Authentication.AuthServices.UserAuth;
using OfferManagement.Application.Authentication.AuthServices.CompanyAuth;
using OfferManagement.Application.Authentication.AuthServices.AdminAuth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using OfferManagement.Common.Workers;
using OfferManagement.Application.EntityServices.Images;
using OfferManagement.Infrastructure.Images;

namespace OfferManagement.Common.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<IOfferService, OfferService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IImageService,  ImageService>();

            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<ICompanyAuthService, CompanyAuthService>();
            services.AddScoped<IUserAuthService, UserAuthService>();
            services.AddScoped<IAdminAuthService, AdminAuthService>();

            services.AddScoped<IPurchaseByUserService, PurchaseByUserService>();
            services.AddScoped<IOfferByCompanyService, OfferByCompanyService>();
            services.AddScoped<ISubscriptionByUserService, SubscriptionByUserService>();

            services.AddHostedService<OfferArchiveWorker>();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IPurchaseRepository, PurchaseRepository>();
            services.AddScoped<IOfferRepository, OfferRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();


            services.AddScoped<IPasswordHasher, PasswordHasher>();

            return services;
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];

            services.Configure<JWTConfiguration>(jwtSettings);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        }
    }
}
