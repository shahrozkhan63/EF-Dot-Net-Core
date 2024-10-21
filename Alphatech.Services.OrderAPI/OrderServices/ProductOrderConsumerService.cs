using Alphatech.Services.OrderAPI.Models.Dto;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Alphatech.Services.OrderAPI.Repository;
using Alphatech.Services.OrderAPI.Models;

namespace Alphatech.Services.OrderAPI.OrderServices
{
    public class ProductOrderConsumerService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;  // Use this to create a scope for scoped services
        private readonly ILogger<ProductOrderConsumerService> _logger;
        private IConnection _connection;
        private IModel _channel;

        // RabbitMQ settings
        private readonly string _queueName = "order_queue";
        private readonly string _exchangeName = "product_exchange";
        private readonly Dictionary<string, Action<string>> _routingKeyHandlers;

        public ProductOrderConsumerService(ILogger<ProductOrderConsumerService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;

            // Initialize routing key handlers dictionary
            _routingKeyHandlers = new Dictionary<string, Action<string>>
            {
                { "order_created", ProcessOrderCreated },
                { "order_canceled", ProcessOrderCanceled }
            };

            InitializeRabbitMQ();
        }

        // Initialize RabbitMQ: Declare exchange and bind queue to it
        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare the exchange (use the appropriate type for your scenario)
            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct, durable: true);

            // Declare the queue
            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);

            // Bind the queue to the exchange with all the routing keys
            foreach (var routingKey in _routingKeyHandlers.Keys)
            {
                _channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: routingKey);
            }

            _logger.LogInformation($"RabbitMQ consumer initialized. Listening on queue '{_queueName}' bound to exchange '{_exchangeName}'.");
            Console.WriteLine($"RabbitMQ consumer initialized. Listening on queue '{_queueName}' bound to exchange '{_exchangeName}'.");

        }

        // Listen for messages and process them
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Process based on the routing key
                if (_routingKeyHandlers.ContainsKey(ea.RoutingKey))
                {
                    _routingKeyHandlers[ea.RoutingKey](message);  // Call the corresponding handler
                }
                else
                {
                    _logger.LogWarning($"Unknown routing key: {ea.RoutingKey}");
                    Console.WriteLine($"Unknown routing key: {ea.RoutingKey}");
                }
            };

            // Start consuming messages from the queue
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        // Handle the message processing for 'order_created'
        private async void ProcessOrderCreated(string message)
        {
            

            var product = JsonSerializer.Deserialize<ProductDto>(message);
            _logger.LogInformation($"Processing 'order_created' message: {message}");

            // Use a new scope to resolve the IOrderRepository
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

                // Generate random values for demonstration
                Random rng = new Random();

                // Create an order item
                OrderItem orderItem = new()
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductPrice = product.Price,
                    Quantity = rng.Next(1, 100)  // Example quantity between 1 and 100
                };

                // Create a new order and associate the order item with it
                var newOrder = new Order
                {
                    OrderNumber = GenerateOrderNumber(),
                    CustomerName = GenerateCustomerName(),
                    OrderDate = DateTime.Now,
                    OrderItems = new List<OrderItem> { orderItem }  // Initialize with the created order item
                };

                // Save the order to the repository (this will also save the order item)
                await orderRepository.CreateOrder(newOrder);

                _logger.LogInformation("Order created and saved to the database.");
            }

            // Log and output the product details for confirmation
         
            _logger.LogInformation($"Received Product: Id={product.Id}, Name={product.Name}, Price={product.Price}, " +
                             $"Description={product.Description}, Category={product.CategoryName}, ImageURL={product.ImageURL}");

            Console.WriteLine($"Received Product: Id={product.Id}, Name={product.Name}, Price={product.Price}, " +
                             $"Description={product.Description}, Category={product.CategoryName}, ImageURL={product.ImageURL}");
        }


        // Handle the message processing for 'order_canceled'
        private void ProcessOrderCanceled(string message)
        {
            var product = JsonSerializer.Deserialize<ProductDto>(message);

            _logger.LogInformation($"Processing 'order_canceled' message: {message}");
            _logger.LogInformation($"Received Product: Id={product.Id}, Name={product.Name}, Price={product.Price}, " +
                            $"Description={product.Description}, Category={product.CategoryName}, ImageURL={product.ImageURL}");

            Console.WriteLine($"Received Product: Id={product.Id}, Name={product.Name}, Price={product.Price}, " +
                           $"Description={product.Description}, Category={product.CategoryName}, ImageURL={product.ImageURL}");
        }

        public static string GenerateOrderNumber()
        {
            // Prefix for the order number
            string prefix = "ORD";

            // Generate a random number between 10000 and 99999 (5 digits)
            Random random = new Random();
            int randomNumber = random.Next(10000, 99999);

            // Combine prefix with the random number
            return $"{prefix}{randomNumber}";
        }

        private static readonly List<string> FirstNames = new List<string>
    {
        "John", "Emily", "Michael", "Sarah", "David", "Jessica", "James", "Linda", "Daniel", "Sophia"
    };

        private static readonly List<string> LastNames = new List<string>
    {
        "Smith", "Johnson", "Brown", "Williams", "Jones", "Miller", "Davis", "Garcia", "Rodriguez", "Martinez"
    };

        public static string GenerateCustomerName()
        {
            Random random = new Random();

            // Select a random first name
            string firstName = FirstNames[random.Next(FirstNames.Count)];

            // Select a random last name
            string lastName = LastNames[random.Next(LastNames.Count)];

            // Combine first and last names
            return $"{firstName} {lastName}";
        }

        // Dispose the connection and channel when done
        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
