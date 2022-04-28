using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Tools;
using MyLab.DockerPeeker.Tools.CgroupsV1;
using MyLab.DockerPeeker.Tools.CgroupsV2;

namespace MyLab.DockerPeeker.Services
{
    public interface IContainerMetricsProviderRegistry
    {
        Task<IContainerMetricsProvider[]> ProvideAsync();
    }

    class ContainerMetricsProviderRegistry : IContainerMetricsProviderRegistry
    {
        private readonly ICGroupDetector _cGroupDetector;

        private Lazy<IContainerMetricsProvider[]> _lazyProvidesV1;
        private Lazy<IContainerMetricsProvider[]> _lazyProvidesV2;

        public ContainerMetricsProviderRegistry(
            IFileContentProviderV1 fileContentProvider1,
            IFileContentProviderV2 fileContentProvider2,
            ICGroupDetector cGroupDetector)
        {
            _cGroupDetector = cGroupDetector;

            _lazyProvidesV1 = new Lazy<IContainerMetricsProvider[]>(() =>
                new IContainerMetricsProvider[]
                {
                    new CpuStatContainerMetricsProviderV1(fileContentProvider1),
                    new MemStatContainerMetricsProviderV1(fileContentProvider1),
                    new BlkStatContainerMetricsProviderV1(fileContentProvider1),
                    new NetStatCmProvider(fileContentProvider1),
                });

            _lazyProvidesV2 = new Lazy<IContainerMetricsProvider[]>(() =>
                new IContainerMetricsProvider[]
                {
                    new CpuStatContainerMetricsProviderV2(fileContentProvider2),
                    new MemStatContainerMetricsProviderV2(fileContentProvider2),
                    new IoStatContainerMetricsProviderV2(fileContentProvider2),
                    new NetStatCmProvider(fileContentProvider2),
                });
        }

        public async Task<IContainerMetricsProvider[]> ProvideAsync()
        {
            var cgroupVer = await _cGroupDetector.GetCGroupVersionAsync();

            switch (cgroupVer)
            {
                case CGroupVersion.V1:
                    return _lazyProvidesV1.Value;
                case CGroupVersion.V2:
                    return _lazyProvidesV2.Value;
                default:
                    throw new ArgumentOutOfRangeException("cgroup-version");
            }
        }
    }
}