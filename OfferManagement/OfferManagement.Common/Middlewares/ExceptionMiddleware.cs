using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using OfferManagement.Application.Exceptions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;

namespace OfferManagement.Common.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            response.StatusCode = exception switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                AlreadyExistsException => (int)HttpStatusCode.Conflict,
                SecurityTokenException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var errorResponse = new
            {
                StatusCode = response.StatusCode,
                Message = exception.Message
            };

            Log.Error(exception, "An error occurred: {Message}", exception.Message);

            return response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }

    }

}
