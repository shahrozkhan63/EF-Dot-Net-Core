using Alphatech.Services.OrderAPI.Models.Dto;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Alphatech.Services.OrderAPI.RabbitMQ
{
    public class ProductConsumer
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly string _exchangeName = "product_exchange"; // Your exchange name
        private readonly string _queueName = "order_queue"; // Your queue name
        private readonly ILogger<ProductConsumer> _logger; // Add this line
        public ProductConsumer(IConnectionFactory connectionFactory, ILogger<ProductConsumer> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            StartListening(); // Start listening for messages on initialization
        }

        public void StartListening()
        {
            var connection = _connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            // Declare the exchange and queue, and bind them
            channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct, durable: true, autoDelete: false);
            channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind(_queueName, _exchangeName, "product"); // Ensure routing key matches

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                // Handle potential deserialization issues
                try
                {
                    var product = JsonSerializer.Deserialize<ProductDto>(json);

                    if (product != null) // Ensure the product is not null
                    {
                        // Log the received product information to NLog
                        _logger.LogInformation($"Received Product: Id={product.ProductId}, Name={product.Name}, Price={product.Price}, " +
                                    $"Description={product.Description}, Category={product.CategoryName}, ImageURL={product.ImageURL}");
                    }

                    var routingKey = ea.RoutingKey;
                    Console.WriteLine($" [x] Received '{routingKey}':'{json}'");

                    // Handle the received product (e.g., save to database)
                    // SaveProductToDatabase(product);

                    // Acknowledge that the message has been processed
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"[!] Error deserializing message: {ex.Message}");
                    _logger.LogError(ex, "Error deserializing message: {0}", json); // Log the error
                    // Optionally, you could handle the error by rejecting the message
                    channel.BasicNack(ea.DeliveryTag, false, false); // Reject message and do not requeue
                }
            };

            channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        }
    }
}
