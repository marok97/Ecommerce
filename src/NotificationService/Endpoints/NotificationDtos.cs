namespace NotificationService.Endpoints;

public record OrderConfirmationRequest(
    Guid OrderId,
    string CustomerEmail,
    IEnumerable<NotificationItemDto> Items,
    decimal TotalAmount);

public record NotificationItemDto(
    string Sku,
    string ProductName,
    int Quantity,
    decimal UnitPrice);
