using Microsoft.Extensions.Options;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.Services.Interfaces;

namespace ZonefyDotnet.Services.HostedService
{
    public class RabbitMqConsumerHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly RabbitMQMessageBroker _rabbitMQMessageBroker;
        private readonly IConsumerService _consumerService;

        public RabbitMqConsumerHostedService(
            ILogger<RabbitMqConsumerHostedService> logger,
            IConsumerService consumerService,
            IOptions<RabbitMQMessageBroker> rabbitMQMessageBroker)
        {
            _logger = logger;
            _consumerService = consumerService;
            _rabbitMQMessageBroker = rabbitMQMessageBroker.Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Background service starting...");
            _consumerService.RecieveMessageAsync(_rabbitMQMessageBroker.QueueNotification);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _consumerService.Dispose();
            base.Dispose();
        }
    }
}

//using Microsoft.Extensions.Options;
//using ZonefyDotnet.Helpers;
//using ZonefyDotnet.Services.Interfaces;

//namespace ZonefyDotnet.Services.HostedService
//{
//    public class RabbitMqConsumerHostedService : BackgroundService
//    {
//        private readonly ILogger<RabbitMqConsumerHostedService> _logger;
//        private readonly RabbitMQMessageBroker _rabbitMQMessageBroker;
//        private readonly IServiceScopeFactory _serviceScopeFactory;

//        public RabbitMqConsumerHostedService(
//            ILogger<RabbitMqConsumerHostedService> logger,
//            IServiceScopeFactory serviceScopeFactory,
//            IOptions<RabbitMQMessageBroker> rabbitMQMessageBroker)
//        {
//            _logger = logger;
//            _serviceScopeFactory = serviceScopeFactory;
//            _rabbitMQMessageBroker = rabbitMQMessageBroker.Value;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            stoppingToken.ThrowIfCancellationRequested();
//            _logger.LogInformation("Background service starting...");

//            // Create a new scope for each message handling to resolve IConsumerService
//            using (var scope = _serviceScopeFactory.CreateScope())
//            {
//                var consumerService = scope.ServiceProvider.GetRequiredService<IConsumerService>();
//                consumerService.RecieveMessage(_rabbitMQMessageBroker.QueueNotification);
//            }
//        }

//        public override void Dispose()
//        {
//            base.Dispose();
//        }
//    }
//}
