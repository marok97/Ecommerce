namespace NotificationService.Endpoints;

public static class NotificationHandlers
{
    public static IResult Health()
    {
        return Results.Ok(new { status = "healthy" });
    }

    public static IResult SendOrderConfirmation(
        OrderConfirmationRequest request,
        ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("NotificationHandlers");
        logger.LogInformation(
            "Order confirmation notification for order {OrderId} sent to {CustomerEmail}. Total: {TotalAmount}",
            request.OrderId,
            request.CustomerEmail,
            request.TotalAmount);

        return Results.Ok(new { status = "sent" });
    }
}
