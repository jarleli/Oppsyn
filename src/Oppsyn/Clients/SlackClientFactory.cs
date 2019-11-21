using Microsoft.Extensions.DependencyInjection;
using System;

namespace Oppsyn.SlackClients
{
    public class SlackClientFactory : ISlackClientFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SlackClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }

        public ISlackFileClient CreateileClient()
        {
            return _serviceProvider.GetRequiredService<ISlackFileClient>();
        }

        public ISlackMessageClient CreateMessageClient()
        {
            return _serviceProvider.GetRequiredService<ISlackMessageClient>();        
        }
    }

    public interface ISlackClientFactory
    {
        public ISlackMessageClient CreateMessageClient();
        public ISlackFileClient CreateileClient();

    }

}
