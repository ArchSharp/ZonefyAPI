using Application.Helpers;

namespace ZonefyDotnet.Services.Interfaces
{
    public interface IConsumerService : IAutoDependencyService, IDisposable
    {
        public void RecieveMessage(string queue);
    }
}