using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RoomReserve.API.Models.Exceptions;

namespace RoomReserve.API.Middlewares
{
    public class GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
        {
            var (statusCode, title) = exception switch
            {
                ServiceNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
                DomainException => (StatusCodes.Status400BadRequest, "Bad Request"),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                logger.LogError(exception, "Unhandled exception: {Title}", title);
            }
            else
            {
                logger.LogWarning(exception, "Handled exception: {Title}", title);
            }

            httpContext.Response.StatusCode = statusCode;

            await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = statusCode == 500 ? "An internal server error occurred." : exception.Message,
                    Instance = httpContext.Request.Path
                }
            });

            return true;
        }
    }
}
