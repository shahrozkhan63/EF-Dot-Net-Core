namespace APIGateway.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory _clientFactory;

        public OrderService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> GetOrderById(int orderId)
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7131/api/orders/{orderId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }
    }

}
