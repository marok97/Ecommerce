namespace OrderService.Clients;

public interface IInventoryServiceClient
{
    Task ReserveAsync(string sku, int quantity);
    Task ReleaseAsync(string sku, int quantity);
    Task ConfirmAsync(string sku, int quantity);
}
