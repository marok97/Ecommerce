using ProductService.Services;

namespace ProductService.Endpoints;

public static class ProductHandlers
{
    public static async Task<IResult> GetAll(
        IProductService productService,
        int page = 1,
        int pageSize = 20,
        string? category = null)
    {
        var result = await productService.GetAll(page, pageSize, category);
        return Results.Ok(result);
    }

    public static async Task<IResult> GetById(Guid productId, IProductService productService)
    {
        var product = await productService.GetById(productId);
        return Results.Ok(product);
    }

    public static async Task<IResult> GetBySku(string sku, IProductService productService)
    {
        var product = await productService.GetBySku(sku);
        return Results.Ok(product);
    }

    public static async Task<IResult> Create(ProductCreate request, IProductService productService)
    {
        var product = await productService.Create(request);
        return Results.Created($"/api/products/{product.Id}", product);
    }

    public static async Task<IResult> Update(Guid productId, ProductUpdate request, IProductService productService)
    {
        var product = await productService.Update(productId, request);
        return Results.Ok(product);
    }

    public static async Task<IResult> Delete(Guid productId, IProductService productService)
    {
        await productService.Delete(productId);
        return Results.NoContent();
    }
}
