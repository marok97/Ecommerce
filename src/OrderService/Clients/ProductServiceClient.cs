using Ecommerce.Shared.Exceptions;
using System.Net.Http.Json;

namespace OrderService.Clients;

public class ProductServiceClient(HttpClient httpClient, ILogger<ProductServiceClient> logger)
    : IProductServiceClient
{
    public async Task<ProductDto> GetBySkuAsync(string sku)
    {
        logger.LogInformation("Looking up product with SKU {Sku}", sku);

        var response = await httpClient.GetAsync($"/api/products/sku/{sku}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            throw new NotFoundException($"Product with SKU '{sku}' not found");

        response.EnsureSuccessStatusCode();

        var product = await response.Content.ReadFromJsonAsync<ProductDto>()
            ?? throw new InvalidOperationException($"Failed to deserialize product response for SKU {sku}");

        return product;
    }
}
