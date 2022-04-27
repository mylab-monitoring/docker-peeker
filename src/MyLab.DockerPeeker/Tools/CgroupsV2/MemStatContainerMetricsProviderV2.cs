using System.Collections.Generic;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;

namespace MyLab.DockerPeeker.Tools.CgroupsV2
{
    public class MemStatContainerMetricsProviderV2 : IContainerMetricsProvider
    {
        private readonly IFileContentProviderV2 _fileContentProvider;

        public MemStatContainerMetricsProviderV2(IFileContentProviderV2 fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId, string pid)
        {
            return null;
            //return new[]
            //{
            //    new ContainerMetric(parser.ExtractKey("swap", "Mem swap"), ContainerMetricType.MemSwapMetricType),
            //    new ContainerMetric(parser.ExtractKey("cache", "Mem cache"), ContainerMetricType.MemCacheMetricType),
            //    new ContainerMetric(parser.ExtractKey("rss", "Mem rss"), ContainerMetricType.MemRssMetricType),
            //    new ContainerMetric(parser.ExtractKey("hierarchical_memory_limit", "Mem limit"), ContainerMetricType.MemLimitMetricType),
            //    new ContainerMetric(parser.ExtractKey("hierarchical_memsw_limit", "Mem+Swap limit"), ContainerMetricType.MemSwLimitMetricType),
            //};

        }
    }
}
