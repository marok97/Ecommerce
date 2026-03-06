using OrderService.Services;

namespace OrderService.Endpoints;

public static class OrderHandlers
{
    public static async Task<IResult> GetAll(IOrderService orderService)
    {
        var orders = await orderService.GetAll();
        return Results.Ok(orders);
    }

    public static async Task<IResult> GetById(Guid orderId, IOrderService orderService)
    {
        var order = await orderService.GetById(orderId);
        return Results.Ok(order);
    }

    public static async Task<IResult> Create(CreateOrderRequest request, IOrderService orderService)
    {
        var order = await orderService.CreateOrder(request);
        return Results.Created($"/api/orders/{order.Id}", order);
    }

    public static async Task<IResult> Cancel(Guid orderId, IOrderService orderService)
    {
        var order = await orderService.CancelOrder(orderId);
        return Results.Ok(order);
    }
}
