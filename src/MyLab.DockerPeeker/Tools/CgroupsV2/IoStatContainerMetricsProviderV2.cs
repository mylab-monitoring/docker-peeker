using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;
using MyLab.DockerPeeker.Tools.StatObjectModel;

namespace MyLab.DockerPeeker.Tools.CgroupsV2
{
    class IoStatContainerMetricsProviderV2 : IContainerMetricsProvider
    {
        private readonly IFileContentProviderV2 _fileContentProvider;

        public IoStatContainerMetricsProviderV2(IFileContentProviderV2 fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId, string pid)
        {
            var statContent = await _fileContentProvider.ReadIoStat(containerLongId);

            var stat = BlkIoStat.ParseV2(statContent);
            
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