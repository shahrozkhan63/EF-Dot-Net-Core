using Alphatech.Services.ProductAPI.Models.Dto;
using Alphatech.Services.ProductAPI.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace Alphatech.Services.ProductAPI.RabbitMQ
{
    public class ProductService
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly string validExchange = "product_exchange";

        public ProductService(IOptions<RabbitMqSettings> rabbitMqSettings)
        {
            // Create the connection factory using the provided settings
            var factory = new ConnectionFactory
            {
                HostName = rabbitMqSettings.Value.HostName,
                Port = rabbitMqSettings.Value.Port,
                UserName = rabbitMqSettings.Value.UserName,
                Password = rabbitMqSettings.Value.Password
            };

            // Create a connection and model (channel)
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare the exchange
            _channel.ExchangeDeclare(exchange: validExchange, type: ExchangeType.Direct, durable: true, autoDelete: false);

            // Enable publisher confirms
            _channel.ConfirmSelect();
        }

        public void PublishProduct(ProductDto product)
        {
            // Serialize the Product object to JSON
            var message = JsonConvert.SerializeObject(product);
            var body = Encoding.UTF8.GetBytes(message);

            // Attempt to publish a message
            _channel.BasicPublish(exchange: validExchange, // Use a valid exchange
                                  routingKey: "product",      // Routing key
                                  basicProperties: null,
                                  body: body,
                                  mandatory: true);            // Set mandatory to true

            // Wait for confirmation with a short timeout
            if (_channel.WaitForConfirms(TimeSpan.FromMilliseconds(100)))
            {
                Console.WriteLine("[x] Message successfully confirmed.");
            }
            else
            {
                Console.WriteLine("[!] Message failed to confirm.");
            }
        }

        // Ensure to dispose of the connection and channel when the service is disposed
        public void Dispose()
        {
            if (_channel != null && _channel.IsOpen)
            {
                _channel.Close();
                _channel.Dispose();
            }

            if (_connection != null && _connection.IsOpen)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }
    }

}
