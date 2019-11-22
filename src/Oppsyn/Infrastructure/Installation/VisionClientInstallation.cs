using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Extensions.DependencyInjection;
using Oppsyn.Clients;
using Oppsyn.Models;

namespace Oppsyn.Infrastructure.Installation
{
    public static class VisionClientInstallation
    {
        public static void AddVisionClient(this IServiceCollection services, AzureConfig config)
        {
            services.AddSingleton(new ApiKeyServiceClientCredentials(config.ServiceToken));
            services.AddHttpClient<IComputerVisionClient, ComputerVisionClient>();
            services.AddTransient<IComputerVisionClient, ComputerVisionClient>(s => new ComputerVisionClient(s.GetRequiredService<ApiKeyServiceClientCredentials>()) { Endpoint = config.Endpoint });
            services.AddSingleton<IVisionClientFactory, VisionClientFactory>();
        }

    }
}
