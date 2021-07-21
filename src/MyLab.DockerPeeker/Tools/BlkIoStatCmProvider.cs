using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;

namespace MyLab.DockerPeeker.Tools
{
    class BlkIoStatCmProvider : IContainerMetricsProvider
    {
        private readonly IFileContentProvider _fileContentProvider;
        private readonly ContainerMetricType _blkReadMetricType;
        private readonly ContainerMetricType _blkWriteMetricType;

        public BlkIoStatCmProvider(IFileContentProvider fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;

            var blkMetricType = new ContainerMetricType
            {
                Name = "container_blk_bytes_total",
                Type = "counter",
            };

            _blkReadMetricType = blkMetricType.AddLabel("direction", "read", "Report total input bytes");
            _blkWriteMetricType = blkMetricType.AddLabel("direction", "write", "Report total output bytes");
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