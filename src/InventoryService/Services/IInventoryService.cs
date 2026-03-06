using InventoryService.Endpoints;

namespace InventoryService.Services;

public interface IInventoryService
{
    public Task<InventoryDtos.InventoryListResponse> GetAll();
    public Task<InventoryDtos.InventoryResponse?> GetBySku(string sku);
    public Task<InventoryDtos.InventoryResponse> CreateInventory(InventoryDtos.InventoryCreate inventoryReq);

    public Task<InventoryDtos.InventoryResponse?> UpdateInventory(string sku,
        InventoryDtos.UpdateInventoryRequest inventoryRequest);

    public Task<InventoryDtos.InventoryResponse?> ReserveInventory(string sku,
        InventoryDtos.ReserveRequest inventoryRequest);

    public Task<InventoryDtos.InventoryResponse?> ConfirmInventory(string sku,
        InventoryDtos.ConfirmRequest inventoryRequest);

    public Task<InventoryDtos.InventoryResponse?> ReleaseInventory(string sku,
        InventoryDtos.ReleaseRequest inventoryRequest);
}