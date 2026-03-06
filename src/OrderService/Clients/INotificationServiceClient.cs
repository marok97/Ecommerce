namespace OrderService.Clients;

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

public interface INotificationServiceClient
{
    Task SendOrderConfirmationAsync(OrderConfirmationRequest request);
}
