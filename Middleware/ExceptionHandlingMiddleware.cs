using System.Net;
using System.Text.Json;

namespace BulkInsertAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unexpected error occurred.");
            var statusCode = exception switch
            {
                ArgumentException => (int)HttpStatusCode.BadRequest,  
                _ => (int)HttpStatusCode.InternalServerError 
            };
            var response = new
            {
                message = "An error occurred while processing your request.",
                error = exception.Message, // This can be hidden in production
                statusCode
            };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
