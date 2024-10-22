using OrderManagementUI.Models;
using Microsoft.Extensions.Http;
using System.Net.Http;
using System.Text.Json;
namespace OrderManagementUI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory _clientFactory;

        public OrderService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                PropertyNameCaseInsensitive = true // To handle camelCase from API.
            };

            var client = _clientFactory.CreateClient("OrderAPI");

            // Call the API Gateway route, not the direct Order API
            var response = await client.GetAsync("/order/getorders"); // API Gateway route
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<Order>>>(options);
            return apiResponse?.Result ?? new List<Order>();
        }



        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                PropertyNameCaseInsensitive = true // To handle camelCase from API.
            };

            var client = _clientFactory.CreateClient("OrderAPI");

            // Call the API Gateway route, not the direct Order API
            var response = await client.GetAsync($"/order/getorderbyid/{orderId}"); // API Gateway route
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<Order>>(options);
            return apiResponse?.Result;
        }


        public async Task<bool> CreateOrderAsync(Order order)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                PropertyNameCaseInsensitive = true // To handle camelCase from API.
            };

            var client = _clientFactory.CreateClient("OrderAPI");

            // Call the API Gateway route
            var response = await client.PostAsJsonAsync("/order/createupdateorder", order, options); // API Gateway route
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateOrderAsync(Order order)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                PropertyNameCaseInsensitive = true // To handle camelCase from API.
            };

            var client = _clientFactory.CreateClient("OrderAPI");

            // Call the API Gateway route
            var response = await client.PutAsJsonAsync($"/order/getorderbyid/{order.OrderId}", order, options); // API Gateway route
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var client = _clientFactory.CreateClient("OrderAPI");

            // Call the API Gateway route
            var response = await client.DeleteAsync($"/order/deleteorder/{orderId}"); // API Gateway route
            return response.IsSuccessStatusCode;
        }

    }

}
