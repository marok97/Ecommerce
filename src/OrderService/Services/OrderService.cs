using Ecommerce.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using OrderService.Clients;
using OrderService.Database;
using OrderService.Endpoints;
using OrderService.Entities;

namespace OrderService.Services;

public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly OrderDbContext _db;
    private readonly IProductServiceClient _productClient;
    private readonly IInventoryServiceClient _inventoryClient;
    private readonly INotificationServiceClient _notificationClient;

    public OrderService(
        ILogger<OrderService> logger,
        OrderDbContext db,
        IProductServiceClient productClient,
        IInventoryServiceClient inventoryClient,
        INotificationServiceClient notificationClient)
    {
        _logger = logger;
        _db = db;
        _productClient = productClient;
        _inventoryClient = inventoryClient;
        _notificationClient = notificationClient;
    }

    public async Task<IEnumerable<OrderResponse>> GetAll()
    {
        var orders = await _db.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(ToDto);
    }

    public async Task<OrderResponse> GetById(Guid orderId)
    {
        var order = await _db.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .SingleOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            var msg = $"Order with id {orderId} not found";
            _logger.LogError(msg);
            throw new NotFoundException(msg);
        }

        return ToDto(order);
    }

    public async Task<OrderResponse> CreateOrder(CreateOrderRequest request)
    {
        var items = request.Items.ToList();

        // Step 1: Look up products for all items
        _logger.LogInformation("Looking up products for order from {Email}", request.CustomerEmail);
        var productLookups = new List<(OrderItemRequest Item, ProductDto Product)>();

        foreach (var item in items)
        {
            var product = await _productClient.GetBySkuAsync(item.Sku);
            productLookups.Add((item, product));
        }

        // Step 2: Reserve inventory for each item, tracking for rollback
        var reserved = new List<(string Sku, int Quantity)>();

        try
        {
            foreach (var (item, _) in productLookups)
            {
                await _inventoryClient.ReserveAsync(item.Sku, item.Quantity);
                reserved.Add((item.Sku, item.Quantity));
            }
        }
        catch (Exception ex)
        {
            // Step 3: Rollback all reserved items on failure
            _logger.LogError(ex, "Reservation failed. Rolling back {Count} reserved items", reserved.Count);
            foreach (var (sku, qty) in reserved)
            {
                await _inventoryClient.ReleaseAsync(sku, qty);
            }
            throw;
        }

        // Step 4: Create order record in DB (status = Pending)
        var orderItems = productLookups.Select(x => new OrderItem
        {
            ProductId = x.Product.Id,
            Sku = x.Item.Sku,
            ProductName = x.Product.Name,
            Quantity = x.Item.Quantity,
            UnitPrice = x.Product.Price
        }).ToList();

        var totalAmount = orderItems.Sum(i => i.UnitPrice * i.Quantity);

        var order = new Order
        {
            CustomerEmail = request.CustomerEmail,
            Status = OrderStatus.Pending,
            TotalAmount = totalAmount,
            Items = orderItems
        };

        await _db.Orders.AddAsync(order);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Order {OrderId} created with status Pending", order.Id);

        try
        {
            // Step 5: Confirm inventory for each item
            foreach (var (item, _) in productLookups)
            {
                await _inventoryClient.ConfirmAsync(item.Sku, item.Quantity);
            }

            // Step 6: Send notification
            var notificationRequest = new OrderConfirmationRequest(
                order.Id,
                order.CustomerEmail,
                order.Items.Select(i => new NotificationItemDto(i.Sku, i.ProductName, i.Quantity, i.UnitPrice)),
                order.TotalAmount);

            await _notificationClient.SendOrderConfirmationAsync(notificationRequest);

            // Step 7: Update order status to Confirmed
            order.Status = OrderStatus.Confirmed;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Order {OrderId} confirmed", order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Post-reservation steps failed for order {OrderId}. Marking as Failed", order.Id);
            order.Status = OrderStatus.Failed;
            await _db.SaveChangesAsync();
            throw;
        }

        return ToDto(order);
    }

    public async Task<OrderResponse> CancelOrder(Guid orderId)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .SingleOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            var msg = $"Order with id {orderId} not found";
            _logger.LogError(msg);
            throw new NotFoundException(msg);
        }

        if (order.Status == OrderStatus.Cancelled)
            throw new ConflictException($"Order {orderId} is already cancelled");

        if (order.Status == OrderStatus.Confirmed)
        {
            // Release inventory for confirmed orders
            foreach (var item in order.Items)
            {
                await _inventoryClient.ReleaseAsync(item.Sku, item.Quantity);
            }
        }

        order.Status = OrderStatus.Cancelled;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Order {OrderId} cancelled", orderId);

        return ToDto(order);
    }

    private static OrderResponse ToDto(Order o) =>
        new(
            o.Id,
            o.CustomerEmail,
            o.Status.ToString(),
            o.TotalAmount,
            o.CreatedAt,
            o.UpdatedAt,
            o.Items.Select(i => new OrderItemResponse(i.Id, i.ProductId, i.Sku, i.ProductName, i.Quantity, i.UnitPrice)));
}
