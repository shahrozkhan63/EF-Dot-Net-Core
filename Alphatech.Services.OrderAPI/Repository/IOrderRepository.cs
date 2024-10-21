using Alphatech.Services.OrderAPI.Models;
using Alphatech.Services.OrderAPI.Models.Dto;

namespace Alphatech.Services.OrderAPI.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderDto>> GetOrders();
        Task<IEnumerable<object>> GetDynamicOrders();
        Task<OrderDto> GetOrderById(int orderId);
        Task<Order> CreateUpdateOrder(Order order);
        Task<int> CreateOrder(Order order);
        Task<bool> DeleteOrder(int orderId);
        Task<int> TestConnectionAsync();
        Task<int> CreateOrderItem(OrderItem order);

    }
}
