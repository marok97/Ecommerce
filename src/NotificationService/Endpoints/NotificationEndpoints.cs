namespace NotificationService.Endpoints;

public static class NotificationEndpoints
{
    public static RouteGroupBuilder MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", NotificationHandlers.Health);

        var group = app.MapGroup("/api/notifications")
            .WithTags("Notifications");

        group.MapPost("/order-confirmation", NotificationHandlers.SendOrderConfirmation);

        return group;
    }
}
