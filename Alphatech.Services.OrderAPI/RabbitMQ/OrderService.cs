using Alphatech.Services.OrderAPI.Models.Dto;
using Alphatech.Services.OrderAPI.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using Alphatech.Services.OrderAPI.Models.Dto;

namespace Alphatech.Services.OrderAPI.RabbitMQ
{
    public class OrderService
    {
        private readonly IModel _channel;

        public OrderService(IOptions<RabbitMqSettings> rabbitMqSettings)
        {
            var factory = new ConnectionFactory() { HostName = rabbitMqSettings.Value.HostName };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "orderQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void PublishOrder(OrderDto order)
        {
            var message = JsonConvert.SerializeObject(order);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "", routingKey: "orderQueue", basicProperties: null, body: body);
            Console.WriteLine($" [x] Sent {message}");
        }
    }

}
