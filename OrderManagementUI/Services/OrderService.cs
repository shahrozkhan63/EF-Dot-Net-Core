using OrderManagementUI.Models;
using Microsoft.Extensions.Http;
using System.Net.Http;
using System.Text.Json;
using OrderManagementUI.ViewModels;
namespace OrderManagementUI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory _clientFactory;

        public OrderService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IEnumerable<OrderViewModel>> GetOrdersAsync()
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

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<OrderViewModel>>>(options);
            return apiResponse?.Result ?? new List<OrderViewModel>();
        }

        public async Task<IEnumerable<OrderViewModel>> GetOrdersItemsAsync()
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

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<OrderViewModel>>>(options);
            return apiResponse?.Result ?? new List<OrderViewModel>();
        }



        public async Task<OrderViewModel?> GetOrderByIdAsync(int orderId)
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

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<OrderViewModel>>(options);
            return apiResponse?.Result;
        }


        public async Task<bool> CreateOrderAsync(OrderViewModel orderViewModel)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                PropertyNameCaseInsensitive = true // To handle camelCase from API.
            };

            var orderJson = JsonSerializer.Serialize(orderViewModel, options);
            Console.WriteLine(orderJson); // Log to console or use a logging framework


            var client = _clientFactory.CreateClient("OrderAPI");

            // Call the API Gateway route
            var response = await client.PostAsJsonAsync("/order/createupdateorder", orderViewModel, options); // API Gateway route
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateOrderAsync(OrderViewModel orderViewModel)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                PropertyNameCaseInsensitive = true // To handle camelCase from API.
            };

            var client = _clientFactory.CreateClient("OrderAPI");

            // Call the API Gateway route
            var response = await client.PostAsJsonAsync("/order/createupdateorder", orderViewModel, options); // API Gateway route
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
