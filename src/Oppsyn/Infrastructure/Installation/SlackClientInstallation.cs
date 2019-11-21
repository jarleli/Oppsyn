using Microsoft.Extensions.DependencyInjection;
using Oppsyn.SlackClients;
using System.Net.Http.Headers;

namespace Oppsyn.Infrastructure.Installation
{
    public static class SlackClientInstallation
    {
        public static void InstallSlackClients(this IServiceCollection services, BotConfig config)
        {
            services.AddHttpClient<ISlackMessageClient, SlackMessageClient>(c =>
                                        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.SlackApiKey));
            services.AddHttpClient<ISlackFileClient, SlackFileClient>(c =>
                    c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.SlackApiKey));
            services.AddSingleton<ISlackClientFactory, SlackClientFactory>();
        }

    }
}
