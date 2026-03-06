using NotificationService.Endpoints;
using NotificationService.Exceptions.ExceptionHandlers;

namespace NotificationService;

public static class ServiceExtension
{
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapNotificationEndpoints();
        return app;
    }

    public static void ConfigureExceptionHandlers(this IServiceCollection services)
    {
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddProblemDetails();
    }
}
