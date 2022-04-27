using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;
using MyLab.DockerPeeker.Tools.StatObjectModel;

namespace MyLab.DockerPeeker.Tools.CgroupsV1
{
    public class MemStatContainerMetricsProviderV1 : IContainerMetricsProvider
    {
        private readonly IFileContentProviderV1 _fileContentProvider;

        public MemStatContainerMetricsProviderV1(IFileContentProviderV1 fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId, string pid)
        {
            var statContent = await _fileContentProvider.ReadMemStat(containerLongId);

            var stat = KeyValueStat.Parse(statContent);

            if (!stat.TryGetValue("swap", out long swapValue))
                throw new InvalidOperationException("'swap' parameter not found");

            if (!stat.TryGetValue("cache", out long cacheValue))
                throw new InvalidOperationException("'cache' parameter not found");

            if (!stat.TryGetValue("rss", out long rssValue))
                throw new InvalidOperationException("'rss' parameter not found");

            if (!stat.TryGetValue("hierarchical_memory_limit", out long memoryLimitValue))
                throw new InvalidOperationException("'hierarchical_memory_limit' parameter not found");

            if (!stat.TryGetValue("hierarchical_memsw_limit", out long memswLimit))
                throw new InvalidOperationException("'hierarchical_memsw_limit' parameter not found");

            return new[]
            {
                new ContainerMetric(swapValue, ContainerMetricType.MemSwapMetricType),
                new ContainerMetric(cacheValue, ContainerMetricType.MemCacheMetricType),
                new ContainerMetric(rssValue, ContainerMetricType.MemRssMetricType),
                new ContainerMetric(memoryLimitValue, ContainerMetricType.MemLimitMetricType),
                new ContainerMetric(memswLimit, ContainerMetricType.MemSwLimitMetricType),
            };

        }
    }
}
