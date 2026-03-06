using Ecommerce.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Exceptions.ExceptionHandlers;

public class NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not NotFoundException e)
        {
            return false;
        }

        var problemDetails = new ProblemDetails()
        {
            Instance = httpContext.Request.Path,
            Title = "Not found",
            Detail = e.Message,
            Status = StatusCodes.Status404NotFound,
        };
        httpContext.Response.StatusCode = (int)e.StatusCode;
        httpContext.Response.ContentType = "application/json";
        logger.LogError("{ProblemDetail} ", problemDetails);
        problemDetails.Status = httpContext.Response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
