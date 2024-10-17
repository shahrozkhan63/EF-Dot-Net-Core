using Alphatech.Services.OrderAPI.Models.Dto;
using Alphatech.Services.OrderAPI.RabbitMQ;
using System.Text.Json;

namespace Alphatech.Services.OrderAPI.OrderServices
{
    public class GetProductOrderService
    {
        private readonly RabbitMQConsumerService _rabbitMQConsumer;
        private readonly ILogger<ProductConsumer> _logger;

        public GetProductOrderService(RabbitMQConsumerService rabbitMQConsumer, ILogger<ProductConsumer> logger)
        {
            _rabbitMQConsumer = rabbitMQConsumer;
            _logger = logger;
        }

        public void StartConsuming()
        {
           // _rabbitMQConsumer.ConsumeMessage("order_queue", HandleOrderMessage);
        }

        private async Task HandleOrderMessage(string message)
        {
            var product = JsonSerializer.Deserialize<ProductDto>(message);
            // Process the order message
            await Task.Run(() => {
                Console.WriteLine($"Processing Order: {message}");

                _logger.LogInformation($"Received Product: Id={product.Id}, Name={product.Name}, Price={product.Price}, " +
                                  $"Description={product.Description}, Category={product.CategoryName}, ImageURL={product.ImageURL}");

                // Further logic for handling the order
            });
        }
    }
}
