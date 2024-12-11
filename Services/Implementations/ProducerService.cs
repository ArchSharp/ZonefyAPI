using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.Services.Interfaces;

namespace Application.Services.Implementations.RabbitMQMessageBrokerService
{
    public class ProducerService : IProducerService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IConnection _connection;
        private readonly RabbitMQMessageBroker _rabbitMQMessageBroker;

        public ProducerService(
            ILogger<ProducerService> logger,
            IRabbitMQConfig rabbitMQConfig,
            IOptions<RabbitMQMessageBroker> rabbitMQMessageBroker)
        {
            _connection = rabbitMQConfig.CreateRabbitMQConnection(false);
            _logger = logger;
            _rabbitMQMessageBroker = rabbitMQMessageBroker.Value;
        }

        public void SendMessage<T>(T message, string queue)
        {
            using var channel = _connection.CreateModel();
            string exchange = _rabbitMQMessageBroker.QueueNotificationExchange;
            string routingKey = _rabbitMQMessageBroker.QueueNotificationRoutingKey;
            ConfigureChannel(channel, queue, exchange, routingKey);
            //channel.QueueDeclare(queue, false, false, false, null);
            string json = JsonConvert.SerializeObject(message);
            byte[] body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange, routingKey, body: body, basicProperties: ChannelProperties(channel));
        }

        private void ConfigureChannel(IModel channel, string queue, string exchange, string routingKey)
        {
            channel.ExchangeDeclare(exchange, ExchangeType.Topic);
            channel.QueueDeclare(queue, false, false, false, null);
            channel.QueueBind(queue, exchange, routingKey, null);
            channel.BasicQos(0, 1, false);
        }

        private IBasicProperties ChannelProperties(IModel channel)
        {
            var properties = channel.CreateBasicProperties();
            properties.AppId = _rabbitMQMessageBroker.AppID;
            properties.ContentType = "application/json";
            properties.DeliveryMode = 1;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            return properties;
        }

        public void Dispose()
        {
            if (_connection != null && _connection.IsOpen)
                _connection.Close();
        }
    }
}

