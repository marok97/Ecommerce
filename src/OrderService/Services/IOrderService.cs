using OrderService.Endpoints;

namespace OrderService.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderResponse>> GetAll();
    Task<OrderResponse> GetById(Guid orderId);
    Task<OrderResponse> CreateOrder(CreateOrderRequest request);
    Task<OrderResponse> CancelOrder(Guid orderId);
}
