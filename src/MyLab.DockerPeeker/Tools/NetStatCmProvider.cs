using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;
using MyLab.DockerPeeker.Tools.StatObjectModel;

namespace MyLab.DockerPeeker.Tools
{
    class NetStatCmProvider : IContainerMetricsProvider
    {
        private readonly IFileContentProviderV1 _fileContentProvider;

        public NetStatCmProvider(IFileContentProviderV1 fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId, string pid)
        {
            var statStr = await _fileContentProvider.ReadNetStat(pid);

            var stat = NetStat.Parse(statStr);
            
            var receive = stat.Sum(p => p.Value.ReceiveBytes);
            var transmit = stat.Sum(p => p.Value.TransmitBytes);

            return new []
            {
                new ContainerMetric(receive, ContainerMetricType.NetReceiveMetricType), 
                new ContainerMetric(transmit, ContainerMetricType.NetTransmitMetricType), 
            };
        }
    }
}