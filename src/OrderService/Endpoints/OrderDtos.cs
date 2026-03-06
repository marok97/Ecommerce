namespace OrderService.Endpoints;

public record CreateOrderRequest(string CustomerEmail, IEnumerable<OrderItemRequest> Items);

public record OrderItemRequest(string Sku, int Quantity);

public record OrderResponse(
    Guid Id,
    string CustomerEmail,
    string Status,
    decimal TotalAmount,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IEnumerable<OrderItemResponse> Items);

public record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    string Sku,
    string ProductName,
    int Quantity,
    decimal UnitPrice);
