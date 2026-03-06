namespace ProductService.Endpoints;

public record ProductCreate(string Sku, string Name, string? Description, decimal Price, string Category);

public record ProductUpdate(string? Name, string? Description, decimal? Price, string? Category);

public record ProductResponse(
    Guid Id,
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    string Category,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record ProductListResponse(IEnumerable<ProductResponse> Items, int Total, int Page, int PageSize);
