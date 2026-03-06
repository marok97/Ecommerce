using InventoryService.Database;
using InventoryService.Endpoints;
using InventoryService.Entities;
using Ecommerce.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Services;

public class InventoryService : IInventoryService
{
    private readonly ILogger<InventoryService> _logger;
    private readonly InventoryDbContext _db;

    public InventoryService(ILogger<InventoryService> logger, InventoryDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task<InventoryDtos.InventoryListResponse> GetAll()
    {
        _logger.LogInformation("Getting all inventories");
        var items = await _db.Inventories.AsNoTracking().ToListAsync();
        _logger.LogInformation($"Found {items.Count} inventories");
        var dtos = items.Select(i => new InventoryDtos.InventoryResponse(
            i.Id,
            i.ProductId,
            i.Sku,
            i.QuantityAvailable,
            i.QuantityReserved,
            i.WarehouseLocation,
            i.UpdatedAt));

        return new InventoryDtos.InventoryListResponse(dtos);
    }

    public async Task<InventoryDtos.InventoryResponse?> GetBySku(string sku)
    {
        var item = await _db.Inventories.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Sku == sku);

        if (item == null)
        {
            var msg = $"Inventory item with SKU: {sku} does not exist";
            _logger.LogError(msg);
            throw new NotFoundException(msg);
        }

        var itemDto = new InventoryDtos.InventoryResponse(
            item.Id,
            item.ProductId,
            item.Sku,
            item.QuantityAvailable,
            item.QuantityReserved,
            item.WarehouseLocation,
            item.UpdatedAt);

        return itemDto;
    }

    public async Task<InventoryDtos.InventoryResponse> CreateInventory(InventoryDtos.InventoryCreate inventoryReq)
    {
        //Check if exists
        var exists = await _db.Inventories.SingleOrDefaultAsync(x => x.Sku == inventoryReq.Sku);
        if (exists != null)
        {
            throw new ConflictException($"An inventory item with SKU: {inventoryReq.Sku} already exists");
        }

        var newInventory = new Inventory
        {
            ProductId = inventoryReq.ProductId,
            Sku = inventoryReq.Sku,
            QuantityAvailable = inventoryReq.QuantityAvailable,
            QuantityReserved = inventoryReq.QuantityReserved,
            WarehouseLocation = inventoryReq.WarehouseLocation
        };

        await _db.Inventories.AddAsync(newInventory);
        await _db.SaveChangesAsync();


        return await GetBySku(inventoryReq.Sku);
    }

    public async Task<InventoryDtos.InventoryResponse?> UpdateInventory(string sku,
        InventoryDtos.UpdateInventoryRequest inventoryUpdateRequest)
    {
        var inventory = await GetInventoryBySku(sku);
        if (inventory == null) return null;

        inventory.QuantityAvailable = inventoryUpdateRequest.QuantityAvailable;
        inventory.WarehouseLocation = inventoryUpdateRequest.WarehouseLocation;
        inventory.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return await GetBySku(sku);
    }

    public async Task<InventoryDtos.InventoryResponse?> ReserveInventory(string sku,
        InventoryDtos.ReserveRequest reserveRequest)
    {
        var inventory = await GetInventoryBySku(sku);
        if (inventory == null)
        {
            var msg = $"Inventory item with SKU: {sku} does not exist";
            throw new NotFoundException(msg);
        }

        var freeStock = inventory.QuantityAvailable - inventory.QuantityReserved;
        if (freeStock < reserveRequest.Quantity)
        {
            var msg =
                $"Insufficient stock for {inventory.Sku}. Free stock: {freeStock}, Requested: {reserveRequest.Quantity}";
            throw new ConflictException(msg);
        }

        inventory.QuantityReserved += reserveRequest.Quantity;

        await _db.SaveChangesAsync();
        return await GetBySku(sku);
    }

    public async Task<InventoryDtos.InventoryResponse?> ConfirmInventory(string sku,
        InventoryDtos.ConfirmRequest confirmRequest)
    {
        var inventory = await GetInventoryBySku(sku);
        if (inventory == null)
        {
            var msg = $"Inventory item with SKU: {sku} does not exist";
            throw new NotFoundException(msg);
        }

        // Check units reserved
        if (inventory.QuantityReserved < confirmRequest.Quantity)
        {
            var msg =
                $"Cannot confirm {confirmRequest.Quantity} units. Only: {inventory.QuantityReserved} units reserved for sku: {inventory.Sku}";
            throw new ConflictException(msg);
        }

        // Check units available
        if (inventory.QuantityAvailable < confirmRequest.Quantity)
        {
            var msg =
                $"Cannot confirm {confirmRequest.Quantity} units. Only: {inventory.QuantityAvailable} units available for sku: {inventory.Sku}";
            throw new ConflictException(msg);
        }

        inventory.QuantityReserved -= confirmRequest.Quantity;
        inventory.QuantityAvailable -= confirmRequest.Quantity;

        await _db.SaveChangesAsync();
        return await GetBySku(sku);
    }

    public async Task<InventoryDtos.InventoryResponse?> ReleaseInventory(string sku,
        InventoryDtos.ReleaseRequest releaseRequest)
    {
        var inventory = await GetInventoryBySku(sku);
        if (inventory == null)
        {
            var msg = $"Inventory item with SKU: {sku} does not exist";
            throw new NotFoundException(msg);
        }

        // Check units reserved
        if (inventory.QuantityReserved < releaseRequest.Quantity)
        {
            var msg =
                $"Cannot release {releaseRequest.Quantity} units. Only: {inventory.QuantityReserved} units reserved for sku: {inventory.Sku}";
            throw new ConflictException(msg);
        }

        inventory.QuantityReserved -= releaseRequest.Quantity;

        await _db.SaveChangesAsync();
        return await GetBySku(sku);
    }

    private async Task<Inventory?> GetInventoryBySku(string sku)
    {
        return await _db.Inventories.SingleOrDefaultAsync(x => x.Sku == sku);
    }
}