using FluentValidation.AspNetCore;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OfferManagement.Common.Middlewares;
using OfferManagement.Common.Validations;
using OfferManagement.Persistance.Context;
using Serilog;
using OfferManagement.Common.Extensions;
using OfferManagement.Common.Mapping;

namespace OfferManagement
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

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Offers API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by a space and then your JWT token."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] { }
                    }
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.ConfigureJWT(builder.Configuration);

            config.Scan(typeof(MappingConfig).Assembly);

            builder.Services.AddSingleton(config);
            builder.Services.AddScoped<IMapper, ServiceMapper>();

            builder.Services.AddDbContext<OfferManagementContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructure();

            builder.Services.AddValidatorsFromAssemblyContaining<AddOfferRequestValidator>();
            builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();


            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
