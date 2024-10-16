using Alphatech.Services.OrderAPI.Models.Dto;

namespace Alphatech.Services.OrderAPI.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderDto>> GetOrders();
        Task<IEnumerable<object>> GetDynamicOrders();
        Task<OrderDto> GetOrderById(int orderId);
        Task<OrderDto> CreateUpdateOrder(OrderDto order);
        Task<OrderDto> CreateOrder(OrderDto order);
        Task<bool> DeleteOrder(int orderId);
        Task<int> TestConnectionAsync();

    }
}
