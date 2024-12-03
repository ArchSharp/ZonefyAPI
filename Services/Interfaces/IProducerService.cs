using Application.Helpers;

namespace ZonefyDotnet.Services.Interfaces
{
    public interface IProducerService : IAutoDependencyService
    {
        Task SendMessage<T>(T message, string queue);
    }
}
