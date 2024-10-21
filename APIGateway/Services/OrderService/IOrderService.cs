namespace APIGateway.Services.OrderService
{
    public interface IOrderService
    {
        Task<string> GetOrderById(int orderId);
    }
}
