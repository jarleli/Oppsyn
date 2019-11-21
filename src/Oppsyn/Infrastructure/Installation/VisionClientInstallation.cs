using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Extensions.DependencyInjection;
using Oppsyn.Clients;

namespace Oppsyn.Infrastructure.Installation
{
    public static class VisionClientInstallation
    {
        public static void AddVisionClient(this IServiceCollection services, BotConfig config)
        {
            services.AddSingleton(new ApiKeyServiceClientCredentials(config.AzureServiceToken));
            services.AddHttpClient<IComputerVisionClient, ComputerVisionClient>();
            services.AddTransient<IComputerVisionClient, ComputerVisionClient>(s => new ComputerVisionClient(s.GetRequiredService<ApiKeyServiceClientCredentials>()) { Endpoint = config.AzureServiceEndpoint });
            services.AddSingleton<IVisionClientFactory, VisionClientFactory>();
        }

    }
}
