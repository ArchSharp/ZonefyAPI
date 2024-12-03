using Application.Helpers;

namespace ZonefyDotnet.Services.Interfaces
{
    public interface IConsumerService : IAutoDependencyService, IDisposable
    {
        Task RecieveMessageAsync(string queue);
    }
}