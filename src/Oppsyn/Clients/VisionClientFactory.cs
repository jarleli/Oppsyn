using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oppsyn.Clients
{
    public class VisionClientFactory : IVisionClientFactory
    {
        private readonly IServiceProvider _provider;

        public VisionClientFactory(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }
        public IComputerVisionClient CreateVisionClient()
        {
            return _provider.GetRequiredService<IComputerVisionClient>();
        }
    }

    public interface IVisionClientFactory
    {
        public IComputerVisionClient CreateVisionClient();
    }
}
