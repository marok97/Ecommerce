using Ecommerce.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Exceptions.ExceptionHandlers;

public class ConflictExceptionHandler(ILogger<ConflictExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ConflictException e)
        {
            return false;
        }

        var problemDetails = new ProblemDetails()
        {
            Instance = httpContext.Request.Path,
            Title = "Conflict",
            Detail = e.Message,
            Status = StatusCodes.Status409Conflict,
        };
        httpContext.Response.StatusCode = (int)e.StatusCode;
        httpContext.Response.ContentType = "application/json";
        logger.LogError("{ProblemDetail} ", problemDetails);
        problemDetails.Status = httpContext.Response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
