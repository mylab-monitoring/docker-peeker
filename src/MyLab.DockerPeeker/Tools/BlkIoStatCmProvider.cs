using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;

namespace MyLab.DockerPeeker.Tools
{
    class BlkIoStatCmProvider : IContainerMetricsProvider
    {
        private readonly IFileContentProvider _fileContentProvider;

        public BlkIoStatCmProvider(IFileContentProvider fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId, string pid)
        {
            var statContent = await _fileContentProvider.ReadBlkStat(containerLongId);
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
                new ContainerMetric(readBytes, ContainerMetricType.BlkReadMetricType), 
                new ContainerMetric(writeBytes, ContainerMetricType.BlkWriteMetricType), 
            };
        }
    }
}