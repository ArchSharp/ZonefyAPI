using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.Services.Interfaces;

public class ProducerService : IProducerService, IAsyncDisposable, IDisposable
{
    private readonly ILogger _logger;
    private readonly IConnection _connection;
    private readonly RabbitMQMessageBroker _rabbitMQMessageBroker;

    public ProducerService(
        ILogger<ProducerService> logger,
        IRabbitMQConfig rabbitMQConfig,
        IOptions<RabbitMQMessageBroker> rabbitMQMessageBroker)
    {
        _connection = rabbitMQConfig.CreateRabbitMQConnection();
        _logger = logger;
        _rabbitMQMessageBroker = rabbitMQMessageBroker.Value;
    }

    public async Task SendMessage<T>(T message, string queue)
    {
        try
        {
            if (!_connection.IsOpen)
            {
                _logger.LogError("RabbitMQ connection is not open.");
                throw new InvalidOperationException("RabbitMQ connection is closed.");
            }
            await using var channel = await _connection.CreateChannelAsync();
            string exchange = _rabbitMQMessageBroker.QueueNotificationExchange;
            string routingKey = _rabbitMQMessageBroker.QueueNotificationRoutingKey;
            await ConfigureChannelAsync(channel, queue, exchange, routingKey);

            string json = JsonConvert.SerializeObject(message);
            byte[] body = Encoding.UTF8.GetBytes(json);

            var properties = CreateChannelProperties(); // Use synchronous method
            await channel.BasicPublishAsync(exchange: exchange, routingKey: routingKey, mandatory: true, basicProperties: properties, body: body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message to RabbitMQ.");
            throw;
        }
    }

    private async Task ConfigureChannelAsync(IChannel channel, string queue, string exchange, string routingKey)
    {
        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Topic, durable: true);
        await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueBindAsync(queue, exchange, routingKey, null);
        await channel.BasicQosAsync(0, 1, false);
    }

    private BasicProperties CreateChannelProperties()
    {
        var properties = new BasicProperties(); //channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.AppId = _rabbitMQMessageBroker.AppID;
        properties.ContentType = "application/json";
        properties.DeliveryMode = (DeliveryModes)2; // Persistent
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        return properties;
    }

   

    public async ValueTask DisposeAsync()
    {
        if (_connection != null && _connection.IsOpen)
        {
            await _connection.CloseAsync();
        }
    }

    public void Dispose()
    {
        DisposeAsync().GetAwaiter().GetResult();
    }
}
