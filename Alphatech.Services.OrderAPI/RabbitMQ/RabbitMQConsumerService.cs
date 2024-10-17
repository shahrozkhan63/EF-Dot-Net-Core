using Alphatech.Services.OrderAPI.Models.Dto;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Alphatech.Services.OrderAPI.RabbitMQ
{
    public class RabbitMQConsumerService : BackgroundService
    {
        private readonly ILogger<RabbitMQConsumerService> _logger;
        private IConnection _connection;
        private IModel _channel;

        // RabbitMQ settings
        private readonly string _queueName = "order_queue";
        private readonly string _exchangeName = "product_exchange";
        private readonly string _routingKey = "product";  // Can be "#" for wildcard routing

        public RabbitMQConsumerService(ILogger<RabbitMQConsumerService> logger)
        {
            _logger = logger;
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

            // Bind the queue to the exchange with a routing key
            _channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: _routingKey);

            _logger.LogInformation($"RabbitMQ consumer initialized. Listening on queue '{_queueName}' bound to exchange '{_exchangeName}' with routing key '{_routingKey}'.");
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
                _logger.LogInformation($"Received message: {message}");

                // Process the message here
                ProcessMessage(message);
            };

            // Start consuming messages from the queue
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        // Handle the message processing
        private void ProcessMessage(string message)
        {
            // You can add logic to process the message here
            // Example: Parse the message, perform business logic, etc.

            var product = JsonSerializer.Deserialize<ProductDto>(message);

            _logger.LogInformation($"Processing message: {message}");
            _logger.LogInformation($"Received Product: Id={product.Id}, Name={product.Name}, Price={product.Price}, " +
                            $"Description={product.Description}, Category={product.CategoryName}, ImageURL={product.ImageURL}");
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
