using Ecommerce.Shared.Exceptions;
using System.Net.Http.Json;

namespace OrderService.Clients;

public class InventoryServiceClient(HttpClient httpClient, ILogger<InventoryServiceClient> logger)
    : IInventoryServiceClient
{
    public async Task ReserveAsync(string sku, int quantity)
    {
        logger.LogInformation("Reserving {Quantity} units of SKU {Sku}", quantity, sku);

        var response = await httpClient.PostAsJsonAsync(
            $"/api/inventories/{sku}/reserve",
            new { Quantity = quantity });

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            throw new NotFoundException($"Inventory for SKU '{sku}' not found");

        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new ConflictException($"Cannot reserve stock for SKU '{sku}': {body}");
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task ReleaseAsync(string sku, int quantity)
    {
        logger.LogInformation("Releasing {Quantity} units of SKU {Sku}", quantity, sku);

        var response = await httpClient.PostAsJsonAsync(
            $"/api/inventories/{sku}/release",
            new { Quantity = quantity });

        if (!response.IsSuccessStatusCode)
            logger.LogWarning("Failed to release inventory for SKU {Sku}: {Status}", sku, response.StatusCode);
    }

    public async Task ConfirmAsync(string sku, int quantity)
    {
        logger.LogInformation("Confirming {Quantity} units of SKU {Sku}", quantity, sku);

        var response = await httpClient.PostAsJsonAsync(
            $"/api/inventories/{sku}/confirm",
            new { Quantity = quantity });

        response.EnsureSuccessStatusCode();
    }
}
