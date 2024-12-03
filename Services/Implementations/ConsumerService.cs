using System.Text;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ZonefyDotnet.Services.Interfaces;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.DTOs;

namespace ZonefyDotnet.Services.Implementations
{
    public class ConsumerService : IConsumerService, IDisposable
    {
        private readonly IEmailService _emailService;
        private readonly EmailSender _sender;
        private readonly RabbitMQMessageBroker _rabbitMQMessageBroker;
        private readonly IConnection _connection;
        private readonly ILogger<ConsumerService> _logger;

        public ConsumerService(
            IEmailService emailService,
            IRabbitMQConfig rabbitMQConfig,
            ILogger<ConsumerService> logger,
            IOptions<RabbitMQMessageBroker> rabbitMQMessageBroker,
            IOptions<EmailSender> sender)
        {
            _emailService = emailService;
            _rabbitMQMessageBroker = rabbitMQMessageBroker.Value;
            _connection = rabbitMQConfig.CreateRabbitMQConnection();
            _logger = logger;
            _sender = sender.Value;
        }

        public async Task RecieveMessageAsync(string queue)
        {
            try
            {
                await using var channel = await _connection.CreateChannelAsync();

                string exchange = _rabbitMQMessageBroker.QueueNotificationExchange;
                string routingKey = _rabbitMQMessageBroker.QueueNotificationRoutingKey;

                await ConfigureChannelAsync(channel, queue, exchange, routingKey);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (model, eventArgs) =>
                {
                    try
                    {
                        var body = eventArgs.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var notification = JsonConvert.DeserializeObject<Notification<EmailRequest>>(message);

                        if (notification != null)
                        {
                            await HandleMessageAsync(notification);
                            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                            _logger.LogInformation($"Processed message for: {notification.Data.ReceiverEmail}");
                        }
                        else
                        {
                            _logger.LogWarning("Received a null or invalid message.");
                            await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message.");
                        await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true);
                    }
                };

                await channel.BasicConsumeAsync(queue, autoAck: false, consumer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing the consumer.");
                throw;
            }
        }

        private async Task HandleMessageAsync(Notification<EmailRequest> notification)
        {
            string type = notification.Type.ToLower();
            switch (type)
            {
                case "email":
                    _emailService.SendEmailUsingMailKit(notification.Data);
                    break;
                default:
                    _logger.LogWarning($"Unknown message type: {type}");
                    break;
            }
            await Task.CompletedTask;
        }

        private async Task ConfigureChannelAsync(IChannel channel, string queue, string exchange, string routingKey)
        {
            await channel.ExchangeDeclareAsync(exchange, ExchangeType.Topic, durable: true);
            await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await channel.QueueBindAsync(queue, exchange, routingKey, null);
            await channel.BasicQosAsync(0, 1, false);
        }

        public void Dispose()
        {
            if (_connection.IsOpen)
            {
                _connection.CloseAsync().GetAwaiter().GetResult();
            }
        }
    }
}
