using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oppsyn.SlackClients
{
    public class SlackClientFactory : ISlackClientFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SlackClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }

        public ISlackFileClient GetFileClient()
        {
            return _serviceProvider.GetRequiredService<ISlackFileClient>();
        }

        public ISlackMessageClient GetMessageClient()
        {
            return _serviceProvider.GetRequiredService<ISlackMessageClient>();        
        }
    }

    public interface ISlackClientFactory
    {
        public ISlackMessageClient GetMessageClient();
        public ISlackFileClient GetFileClient();

    }

}
