using Application.Helpers;
using RabbitMQ.Client;

namespace ZonefyDotnet.Services.Interfaces
{
    public interface IRabbitMQConfig : IAutoDependencyService
    {
        IConnection CreateRabbitMQConnection(bool async);
    }
}
