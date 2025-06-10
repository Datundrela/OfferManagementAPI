using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OfferManagement.Common.Mapping;
using OfferManagement.Persistance.Context;
using Serilog;
using OfferManagement.Common.Extensions;
using OfferManagement.Common.Middlewares;
using FluentValidation.AspNetCore;
using FluentValidation;
using OfferManagement.Common.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace OfferManagement.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            var config = TypeAdapterConfig.GlobalSettings;

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions
                    {
                        TableName = "Logs",
                        AutoCreateSqlTable = true
                    })
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<OfferManagementContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructure();

            config.Scan(typeof(MappingConfig).Assembly);
            builder.Services.AddSingleton(config);
            builder.Services.AddScoped<IMapper, ServiceMapper>();

            builder.Services.ConfigureJWT(builder.Configuration);

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseMiddleware<TokenMiddleware>();


            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
