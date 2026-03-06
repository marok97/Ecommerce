namespace InventoryService.Endpoints;

public static class InventoryEndpoints
{
    public static RouteGroupBuilder MapInventoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/inventories")
            .WithTags("Inventory");
        group.MapGet("/", InventoryHandlers.GetAll);
        group.MapGet("/{sku}", InventoryHandlers.GetBySku);
        group.MapPost("/", InventoryHandlers.Create);
        group.MapPut("/{sku}", InventoryHandlers.Update);

        group.MapPost("{sku}/reserve", InventoryHandlers.Reserve);
        group.MapPost("{sku}/release", InventoryHandlers.Release);
        group.MapPost("{sku}/confirm", InventoryHandlers.Confirm);
        return group;
    }
}