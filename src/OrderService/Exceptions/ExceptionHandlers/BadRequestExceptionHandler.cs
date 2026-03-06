using Ecommerce.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Exceptions.ExceptionHandlers;

public class BadRequestExceptionHandler(ILogger<BadRequestExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not BadRequestException e)
        {
            return false;
        }

        var problemDetails = new ProblemDetails()
        {
            Instance = httpContext.Request.Path,
            Title = "Bad request",
            Detail = e.Message,
            Status = StatusCodes.Status400BadRequest,
        };
        httpContext.Response.StatusCode = (int)e.StatusCode;
        httpContext.Response.ContentType = "application/json";
        logger.LogError("{ProblemDetail} ", problemDetails);
        problemDetails.Status = httpContext.Response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
