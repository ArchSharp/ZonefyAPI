using System.Text;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.Services.Interfaces;

namespace Application.Services.Implementations.RabbitMQMessageBrokerService
{
    public class ConsumerService : IConsumerService
    {
        private readonly IEmailService _emailService;
        private readonly EmailSender _sender;
        private readonly RabbitMQMessageBroker _rabbitMQMessageBroker;
        private readonly IConnection _connection;
        private readonly ILogger _logger;

        private int failed = 0;

        public ConsumerService(
            IEmailService emailService,
            IRabbitMQConfig rabbitMQConfig,
            ILogger<ConsumerService> logger,
            IOptions<RabbitMQMessageBroker> rabbitMQMessageBroker,
            IOptions<EmailSender> sender)
        {
            _emailService = emailService;
            _rabbitMQMessageBroker = rabbitMQMessageBroker.Value;
            _connection = rabbitMQConfig.CreateRabbitMQConnection(true);
            _logger = logger;
            _sender = sender.Value;
        }

        public void RecieveMessage(string queue)
        {
            var channel = _connection.CreateModel();
            channel = ConfigureChannel(channel, queue);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, eventArgs) =>
            {
                try
                {
                    //if (failed == 0)
                    //{
                    // _logger.LogInformation($"Retrying again {failed}");
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var verificationPayload = JsonConvert.DeserializeObject<Notification<EmailRequest>>(message);
                    HandleMessage(verificationPayload);
                    channel.BasicAck(eventArgs.DeliveryTag, false);
                    _logger.LogInformation($"Message sent to: {verificationPayload.Data.ReceiverEmail}");
                    //}
                }
                catch (Exception ex)
                {
                    failed++;
                    _logger.LogInformation($"Failed to process message: {ex.Message}");

                    // Requeue the failed message
                    channel.BasicNack(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: true);
                    _logger.LogInformation($"Message is back to the queue.");
                }
            };
            channel.BasicConsume(queue, false, consumer);
        }

        private Task HandleMessage(Notification<EmailRequest> message)
        {
            string type = message.Type.ToLower();
            switch (type)
            {
                case "email":
                    _emailService.SendEmailUsingMailKit(message.Data);
                    break;
                default:
                    break;
            }
            return null;
        }

        private IModel ConfigureChannel(IModel channel, string queue)
        {
            string routingKey = _rabbitMQMessageBroker.QueueIdentityRoutingKey;
            string exchange = _rabbitMQMessageBroker.QueueIdentityExchange;
            channel.ExchangeDeclare(exchange, ExchangeType.Topic);
            channel.QueueDeclare(queue, false, false, false, null);
            channel.QueueBind(queue, exchange, routingKey, null);
            channel.BasicQos(0, 1, false);
            return channel;
        }

        public void Dispose()
        {
            if (_connection.IsOpen)
                _connection.Close();
        }
    }
}