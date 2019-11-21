using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Extensions.DependencyInjection;
using Oppsyn.Clients;

namespace Oppsyn.Infrastructure.Installation
{
    public static class MessageHandlerInstallation
    {
        public static void AddMessageHandlers(this IServiceCollection services)
        {
            services.AddSingleton<IMessageHandler, RunAzureVisionOnImageUpload>();
            services.AddSingleton<MessageDispatcher>();
        }

    }
}
