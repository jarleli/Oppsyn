using Microsoft.Extensions.DependencyInjection;
using Oppsyn.Models;
using Oppsyn.SlackClients;
using System.Net.Http.Headers;

namespace Oppsyn.Infrastructure.Installation
{
    public static class SlackClientInstallation
    {
        public static void InstallSlackClients(this IServiceCollection services, SlackConfig config)
        {
            services.AddSingleton<ISlackClientFactory, SlackClientFactory>();

            services.AddHttpClient<ISlackMessageClient, SlackMessageClient>(c =>                                        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.OauthAccessToken));
            services.AddHttpClient<ISlackFileClient, SlackFileClient>(c => c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.OauthAccessToken));
        }

    }
}
