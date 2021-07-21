using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;

namespace MyLab.DockerPeeker.Tools
{
    class BlkIoStatCmProvider : IContainerMetricsProvider
    {
        private readonly IFileContentProvider _fileContentProvider;

        private readonly ContainerMetricType _blkReadMetricType = new ContainerMetricType
        {
            Name = "container_blk_read_bytes_total",
            Type = "counter",
            Description = "Report total input bytes"
        };

        readonly ContainerMetricType _blkWriteMetricType = new ContainerMetricType
        {
            Name = "container_blk_writes_bytes_total",
            Type = "counter",
            Description = "Report total output bytes"

        };

        public BlkIoStatCmProvider(IFileContentProvider fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId, string pid)
        {
            var statContent = await _fileContentProvider.ReadBlkIoServiceBytesStat(containerLongId);
            var parser = StatParser.Create(statContent);

            var prams = parser.ExtractIdentifiable();

            var grouped = prams
                .GroupBy(p => p.Key)
                .ToArray();

            var readBytes = grouped
                .FirstOrDefault(g => g.Key == "Read")
                ?.Sum(r => r.Value) 
                            ?? 0;
            var writeBytes = grouped
                .FirstOrDefault(g => g.Key == "Write")
                ?.Sum(r => r.Value) 
                             ?? 0;

            return new []
            {
                new ContainerMetric(readBytes, _blkReadMetricType), 
                new ContainerMetric(writeBytes, _blkWriteMetricType), 
            };
        }
    }
}