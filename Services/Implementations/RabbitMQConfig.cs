using Domain.Entities.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.Services.Interfaces;

namespace Application.Services.MessageBrokerConfig
{
    public class RabbitMQConfig : IRabbitMQConfig
    {
        private readonly ILogger _logger;
        private readonly RabbitMQMessageBroker _rabbitMQMessageBroker;


        public RabbitMQConfig(
            ILogger<RabbitMQConfig> logger,
            IOptions<RabbitMQMessageBroker> rabbitMQMessageBroker)
        {
            _logger = logger;
            _rabbitMQMessageBroker = rabbitMQMessageBroker.Value;
        }

        public IConnection CreateRabbitMQConnection()
        {
            _logger.LogInformation("Connecting to RabbitMQ");
            ConnectionFactory factory = new()
            {
                HostName = _rabbitMQMessageBroker.RabbitMQHost,
                Password = _rabbitMQMessageBroker.RabbitMQPassword,
                UserName = _rabbitMQMessageBroker.RabbitMQUsername,
                Port = int.Parse(_rabbitMQMessageBroker.RabbitMQPort),
                VirtualHost = _rabbitMQMessageBroker.RabbitMQVirtual,
                AutomaticRecoveryEnabled = true,                
                //DispatchConsumersAsync = async,
            };
            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        }
    }
}