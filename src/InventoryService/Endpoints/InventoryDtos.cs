using System.ComponentModel.DataAnnotations;

namespace InventoryService.Endpoints;

public class InventoryDtos
{
    public record InventoryCreate(
        Guid ProductId,
        string Sku,
        int QuantityAvailable,
        int QuantityReserved,
        string WarehouseLocation);

    public record InventoryListResponse(IEnumerable<InventoryResponse> Items);

    public record InventoryResponse(
        Guid Id,
        Guid ProductId,
        string Sku,
        int QuantityAvailable,
        int QuantityReserved,
        string WarehouseLocation,
        DateTime UpdatedAt);


    public abstract record ReserveRequest(
        [property: Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        int Quantity
    );

    public abstract record ReleaseRequest(
        [property: Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        int Quantity
    );

    public abstract record ConfirmRequest(
        [property: Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        int Quantity
    );

    public abstract record UpdateInventoryRequest(
        [property: Range(0, int.MaxValue, ErrorMessage = "QuantityAvailable cannot be negative")]
        int QuantityAvailable,
        [property: Required]
        [property: StringLength(50, MinimumLength = 1)]
        string WarehouseLocation
    );
}