namespace OrderService.Endpoints;

public static class OrderEndpoints
{
    public static RouteGroupBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

        var group = app.MapGroup("/api/orders")
            .WithTags("Orders");

        group.MapGet("/", OrderHandlers.GetAll);
        group.MapGet("/{orderId:guid}", OrderHandlers.GetById);
        group.MapPost("/", OrderHandlers.Create);
        group.MapPut("/{orderId:guid}/cancel", OrderHandlers.Cancel);

        return group;
    }
}
