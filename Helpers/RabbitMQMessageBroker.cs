namespace ZonefyDotnet.Helpers
{
    public class RabbitMQMessageBroker
    {
        public string AppID { get; set; } = null!;
        public string RabbitMQHost { get; set; } = null!;
        public string RabbitMQUsername { get; set; } = null!;
        public string RabbitMQPassword { get; set; } = null!;
        public string RabbitMQPort { get; set; } = null!;
        public string RabbitMQVirtual { get; set; } = null!;
        public string QueueUser { get; set; } = null!;
        public string QueueNotification { get; set; } = null!;
        public string QueueNotificationExchange { get; set; } = null!;
        public string QueueNotificationRoutingKey { get; set; } = null!;
        public string QueueIdentity { get; set; } = null!;
        public string QueueIdentityRoutingKey { get; set; } = null!;
        public string QueueIdentityExchange { get; set; } = null!;
    }
}
