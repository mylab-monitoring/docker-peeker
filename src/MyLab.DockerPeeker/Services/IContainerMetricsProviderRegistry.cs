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
        private readonly IContainerMetricsProvider[] _providers;

        public ContainerMetricsProviderRegistry(IFileContentProvider fileContentProvider)
        {
            _providers = new IContainerMetricsProvider[]
            {
                new CpuAcctStatCmProvider(fileContentProvider),
                new MemStatCmProvider(fileContentProvider),
            };
        }

        public IEnumerable<IContainerMetricsProvider> Provide()
        {
            return _providers;
        }
    }
}