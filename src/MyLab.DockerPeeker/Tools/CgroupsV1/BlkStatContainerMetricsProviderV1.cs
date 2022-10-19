using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;
using MyLab.DockerPeeker.Tools.StatObjectModel;

namespace MyLab.DockerPeeker.Tools.CgroupsV1
{
    class BlkStatContainerMetricsProviderV1 : IContainerMetricsProvider
    {
        private readonly IFileContentProviderV1 _fileContentProvider;

        public BlkStatContainerMetricsProviderV1(IFileContentProviderV1 fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId, string pid)
        {
            var statContent = await _fileContentProvider.ReadBlkStat(containerLongId);

            var stat = BlkIoStat.ParseV1(statContent);

            var readBytes = stat.Sum(s => s.Value.Read);
            var writeBytes = stat.Sum(s => s.Value.Write);

            return new []
            {
                new ContainerMetric(readBytes, ContainerMetricType.BlkReadMetricType), 
                new ContainerMetric(writeBytes, ContainerMetricType.BlkWriteMetricType), 
            };
        }
    }
}