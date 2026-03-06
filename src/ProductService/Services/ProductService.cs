using Ecommerce.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using ProductService.Database;
using ProductService.Endpoints;
using ProductService.Entities;

namespace ProductService.Services;

public class ProductService : IProductService
{
    private readonly ILogger<ProductService> _logger;
    private readonly ProductDbContext _db;

    public ProductService(ILogger<ProductService> logger, ProductDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task<ProductListResponse> GetAll(int page, int pageSize, string? category)
    {
        pageSize = Math.Min(pageSize, 100);

        var query = _db.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category == category);

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        _logger.LogInformation("Found {Count} products (page {Page}/{PageSize})", items.Count, page, pageSize);

        return new ProductListResponse(items.Select(ToDto), total, page, pageSize);
    }

    public async Task<ProductResponse> GetById(Guid productId)
    {
        var product = await _db.Products.AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            var msg = $"Product with id {productId} not found";
            _logger.LogError(msg);
            throw new NotFoundException(msg);
        }

        return ToDto(product);
    }

    public async Task<ProductResponse> GetBySku(string sku)
    {
        var product = await _db.Products.AsNoTracking()
            .SingleOrDefaultAsync(p => p.Sku == sku);

        if (product == null)
        {
            var msg = $"Product with SKU '{sku}' not found";
            _logger.LogError(msg);
            throw new NotFoundException(msg);
        }

        return ToDto(product);
    }

    public async Task<ProductResponse> Create(ProductCreate request)
    {
        var exists = await _db.Products.AnyAsync(p => p.Sku == request.Sku);
        if (exists)
            throw new ConflictException($"A product with SKU '{request.Sku}' already exists");

        var product = new Product
        {
            Sku = request.Sku,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category
        };

        await _db.Products.AddAsync(product);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Created product {ProductId} with SKU {Sku}", product.Id, product.Sku);

        return ToDto(product);
    }

    public async Task<ProductResponse> Update(Guid productId, ProductUpdate request)
    {
        var product = await _db.Products.SingleOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            var msg = $"Product with id {productId} not found";
            _logger.LogError(msg);
            throw new NotFoundException(msg);
        }

        if (request.Name != null) product.Name = request.Name;
        if (request.Description != null) product.Description = request.Description;
        if (request.Price.HasValue) product.Price = request.Price.Value;
        if (request.Category != null) product.Category = request.Category;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Updated product {ProductId}", productId);

        return ToDto(product);
    }

    public async Task Delete(Guid productId)
    {
        var product = await _db.Products.SingleOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            var msg = $"Product with id {productId} not found";
            _logger.LogError(msg);
            throw new NotFoundException(msg);
        }

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Deleted product {ProductId}", productId);
    }

    private static ProductResponse ToDto(Product p) =>
        new(p.Id, p.Sku, p.Name, p.Description, p.Price, p.Category, p.CreatedAt, p.UpdatedAt);
}
