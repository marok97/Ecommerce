using System.Net.Http.Json;

namespace OrderService.Clients;

public class NotificationServiceClient(HttpClient httpClient, ILogger<NotificationServiceClient> logger)
    : INotificationServiceClient
{
    public async Task SendOrderConfirmationAsync(OrderConfirmationRequest request)
    {
        logger.LogInformation("Sending order confirmation for order {OrderId} to {Email}",
            request.OrderId, request.CustomerEmail);

        var response = await httpClient.PostAsJsonAsync("/api/notifications/order-confirmation", request);

        if (!response.IsSuccessStatusCode)
            logger.LogWarning("Notification failed for order {OrderId}: {Status}", request.OrderId, response.StatusCode);
    }
}
