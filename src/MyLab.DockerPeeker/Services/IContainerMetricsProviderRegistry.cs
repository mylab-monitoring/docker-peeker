using System.Collections.Generic;
using MyLab.DockerPeeker.Tools;

namespace MyLab.DockerPeeker.Services
{
    public interface IContainerMetricsProviderRegistry
    {
        IEnumerable<IContainerMetricsProvider> Provide();
    }

    class ContainerMetricsProviderRegistry : IContainerMetricsProviderRegistry
    {
        private readonly IContainerMetricsProvider[] _providers = 
        {
            new CpuAcctStatCmProvider(),
        };

        public IEnumerable<IContainerMetricsProvider> Provide()
        {
            return _providers;
        }
    }
}