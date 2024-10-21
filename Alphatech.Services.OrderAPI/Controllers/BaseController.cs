using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace Alphatech.Services.OrderAPI.Controllers
{
    public class BaseController : ControllerBase//, IDisposable
    {
        protected IConnection _connection;
        protected IModel _channel;

        public BaseController()
        {
            //var factory = new ConnectionFactory() { HostName = "localhost" };
            //_connection = factory.CreateConnection();
            //_channel = _connection.CreateModel();
        }

        //protected void DeclareExchange(string exchangeName, string exchangeType = "direct", bool durable = true, bool autoDelete = true)
        //{
        //    _channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType, durable: durable, autoDelete);
        //}

        //protected void DeclareQueue(string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false)
        //{
        //    _channel.QueueDeclare(queue: queueName, durable: durable, exclusive: exclusive, autoDelete: autoDelete);
        //}

        //protected void BindQueueToExchange(string queueName, string exchangeName, string routingKey)
        //{
        //    _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);
        //}

        //protected void PublishMessage(string exchange, string routingKey, byte[] body)
        //{
        //    _channel.BasicPublish(exchange: exchange, routingKey: routingKey, body: body);
        //}

        //// Implement IDisposable to clean up RabbitMQ resources
        //public void Dispose()
        //{
        //    if (_channel != null)
        //    {
        //        _channel.Close();
        //        _channel.Dispose();
        //    }

        //    if (_connection != null)
        //    {
        //        _connection.Close();
        //        _connection.Dispose();
        //    }
        //}
    }
}
