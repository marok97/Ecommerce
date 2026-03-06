using ProductService.Endpoints;

namespace ProductService.Services;

public interface IProductService
{
    Task<ProductListResponse> GetAll(int page, int pageSize, string? category);
    Task<ProductResponse> GetById(Guid productId);
    Task<ProductResponse> GetBySku(string sku);
    Task<ProductResponse> Create(ProductCreate request);
    Task<ProductResponse> Update(Guid productId, ProductUpdate request);
    Task Delete(Guid productId);
}
