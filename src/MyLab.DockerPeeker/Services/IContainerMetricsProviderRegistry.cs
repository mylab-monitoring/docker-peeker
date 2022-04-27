using System.Collections.Generic;
using MyLab.DockerPeeker.Tools;
using MyLab.DockerPeeker.Tools.CgroupsV1;

namespace MyLab.DockerPeeker.Services
{
    public interface IContainerMetricsProviderRegistry
    {
        IEnumerable<IContainerMetricsProvider> Provide();
    }

    class ContainerMetricsProviderRegistry : IContainerMetricsProviderRegistry
    {
        private readonly IContainerMetricsProvider[] _providers;

        public ContainerMetricsProviderRegistry(IFileContentProviderV1 fileContentProvider)
        {
            _providers = new IContainerMetricsProvider[]
            {
                new CpuStatContainerMetricsProviderV1(fileContentProvider),
                new MemStatContainerMetricsProviderV1(fileContentProvider),
                new BlkStatContainerMetricsProviderV1(fileContentProvider),
                new NetStatCmProvider(fileContentProvider),
            };
        }

        public IEnumerable<IContainerMetricsProvider> Provide()
        {
            return _providers;
        }
    }
}