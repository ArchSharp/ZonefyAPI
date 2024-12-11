using Application.Helpers;

namespace ZonefyDotnet.Services.Interfaces
{
    public interface IProducerService : IAutoDependencyService
    {
        void SendMessage<T>(T message, string queue);
    }
}
