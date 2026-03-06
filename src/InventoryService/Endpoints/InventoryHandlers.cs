using InventoryService.Services;

namespace InventoryService.Endpoints;

public static class InventoryHandlers
{
    public static async Task<IResult> GetAll(IInventoryService inventoryService)
    {
        var items = await inventoryService.GetAll();
        return Results.Ok(items);
    }

    public static async Task<IResult> GetBySku(string sku, IInventoryService inventoryService)
    {
        var item = await inventoryService.GetBySku(sku);
        return item == null ? Results.NotFound() : Results.Ok(item);
    }

    public static async Task<IResult> Create(InventoryDtos.InventoryCreate inventoryReq,
        IInventoryService inventoryService)
    {
        var item = await inventoryService.CreateInventory(inventoryReq);

        return Results.Ok(item);
    }

    public static async Task<IResult> Update(string sku, InventoryDtos.UpdateInventoryRequest inventoryUpdateReq,
        IInventoryService inventoryService)
    {
        var item = await inventoryService.UpdateInventory(sku, inventoryUpdateReq);
        return Results.Ok(item);
    }

    public static async Task<IResult> Reserve(string sku, InventoryDtos.ReserveRequest reserveRequest,
        IInventoryService inventoryService)
    {
        var item = await inventoryService.ReserveInventory(sku, reserveRequest);
        return Results.Ok(item);
    }

    public static async Task<IResult> Release(string sku, InventoryDtos.ReleaseRequest releaseRequest,
        IInventoryService inventoryService)
    {
        var item = await inventoryService.ReleaseInventory(sku, releaseRequest);
        return Results.Ok(item);
    }

    public static async Task<IResult> Confirm(string sku, InventoryDtos.ConfirmRequest confirmRequest,
        IInventoryService inventoryService)
    {
        var item = await inventoryService.ConfirmInventory(sku, confirmRequest);
        return Results.Ok(item);
    }
}