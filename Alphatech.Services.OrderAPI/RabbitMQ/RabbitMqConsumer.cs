using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Alphatech.Services.OrderAPI.Models.Dto;
using System.Text;
using Newtonsoft.Json;
using Alphatech.Services.OrderAPI.Models;
using System.Threading.Channels;

namespace Alphatech.Services.OrderAPI.RabbitMQ
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly IModel _channel;

        public RabbitMqConsumer(IOptions<RabbitMqSettings> rabbitMqSettings)
        {
            var factory = new ConnectionFactory() { HostName = rabbitMqSettings.Value.HostName };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "productQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => _channel.Close());
            _channel.ExchangeDeclare(exchange: "product_exchange", type: ExchangeType.Fanout);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var product = JsonConvert.DeserializeObject<ProductDto>(message);
                Console.WriteLine($" [x] Received {product.Name}");
                // Here, you can handle the product data (e.g., create an order item, etc.)
            };

            _channel.BasicConsume(queue: "productQueue", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }

}
