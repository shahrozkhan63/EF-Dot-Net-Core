using OrderManagementUI.Models;
using OrderManagementUI.ViewModels;

namespace OrderManagementUI.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderViewModel>> GetOrdersAsync();
        Task<OrderViewModel> GetOrderByIdAsync(int id);
        Task<bool> CreateOrderAsync(OrderViewModel orderViewModel);
        Task<bool> UpdateOrderAsync(OrderViewModel orderViewModel);
        Task<bool> DeleteOrderAsync(int orderId);
    }
}
