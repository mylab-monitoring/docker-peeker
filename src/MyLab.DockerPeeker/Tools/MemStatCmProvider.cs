using System.Collections.Generic;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;

namespace MyLab.DockerPeeker.Tools
{
    public class MemStatCmProvider : IContainerMetricsProvider
    {
        private readonly IFileContentProvider _fileContentProvider;

        private readonly ContainerMetricType _memSwapMetricType;
        private readonly ContainerMetricType _memCacheMetricType;
        private readonly ContainerMetricType _memRssMetricType;
        private readonly ContainerMetricType _memLimitMetricType;
        readonly ContainerMetricType _memSwLimitMetricType;

        public MemStatCmProvider(IFileContentProvider fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;

            var memParameterMetricType = new ContainerMetricType
            {
                Name = "container_mem_bytes",
                Type = "gauge"
            };

            _memCacheMetricType = memParameterMetricType.AddLabel("type", "cache", "The amount of memory used by the processes of this control group that can be associated precisely with a block on a block device");

            _memRssMetricType = memParameterMetricType.AddLabel("type", "rss", "The amount of memory that doesn’t correspond to anything on disk: stacks, heaps, and anonymous memory maps");

            _memSwapMetricType = memParameterMetricType.AddLabel("type", "swap", "The amount of swap currently used by the processes in this cgroup");

            var memLimitMetricType = new ContainerMetricType
            {
                Name = "container_mem_limit_bytes",
                Type = "gauge"
            };

            _memLimitMetricType = memLimitMetricType.AddLabel("type", "ram", "Indicates the maximum amount of physical memory that can be used by the processes of this control group");

            _memSwLimitMetricType = memLimitMetricType.AddLabel("type", "ramswap", "Indicates the maximum amount of RAM+swap that can be used by the processes of this control group");
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId, string pid)
        {
            var statContent = await _fileContentProvider.ReadMemStat(containerLongId);
            var parser = StatParser.Create(statContent);

            return new[]
            {
                new ContainerMetric(parser.ExtractKey("swap", "Mem swap"), _memSwapMetricType),
                new ContainerMetric(parser.ExtractKey("cache", "Mem cache"), _memCacheMetricType),
                new ContainerMetric(parser.ExtractKey("rss", "Mem rss"), _memRssMetricType),
                new ContainerMetric(parser.ExtractKey("hierarchical_memory_limit", "Mem limit"), _memLimitMetricType),
                new ContainerMetric(parser.ExtractKey("hierarchical_memsw_limit", "Mem+Swap limit"), _memSwLimitMetricType),
            };

        }
    }
}
