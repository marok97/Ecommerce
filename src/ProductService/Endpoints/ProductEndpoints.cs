namespace ProductService.Endpoints;

public static class ProductEndpoints
{
    public static RouteGroupBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet("/", ProductHandlers.GetAll);
        group.MapGet("/sku/{sku}", ProductHandlers.GetBySku);
        group.MapGet("/{productId:guid}", ProductHandlers.GetById);
        group.MapPost("/", ProductHandlers.Create);
        group.MapPut("/{productId:guid}", ProductHandlers.Update);
        group.MapDelete("/{productId:guid}", ProductHandlers.Delete);

        return group;
    }
}
