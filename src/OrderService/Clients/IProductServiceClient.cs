namespace OrderService.Clients;

public record ProductDto(Guid Id, string Sku, string Name, decimal Price);

public interface IProductServiceClient
{
    Task<ProductDto> GetBySkuAsync(string sku);
}
