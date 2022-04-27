using System.Collections.Generic;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;

namespace MyLab.DockerPeeker.Tools
{
    public class MemStatCmProvider : IContainerMetricsProvider
    {
        private readonly IFileContentProvider _fileContentProvider;

        public MemStatCmProvider(IFileContentProvider fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId, string pid)
        {
            var statContent = await _fileContentProvider.ReadMemStat(containerLongId);
            var parser = StatParser.Create(statContent);

            return new[]
            {
                new ContainerMetric(parser.ExtractKey("swap", "Mem swap"), ContainerMetricType.MemSwapMetricType),
                new ContainerMetric(parser.ExtractKey("cache", "Mem cache"), ContainerMetricType.MemCacheMetricType),
                new ContainerMetric(parser.ExtractKey("rss", "Mem rss"), ContainerMetricType.MemRssMetricType),
                new ContainerMetric(parser.ExtractKey("hierarchical_memory_limit", "Mem limit"), ContainerMetricType.MemLimitMetricType),
                new ContainerMetric(parser.ExtractKey("hierarchical_memsw_limit", "Mem+Swap limit"), ContainerMetricType.MemSwLimitMetricType),
            };

        }
    }
}
