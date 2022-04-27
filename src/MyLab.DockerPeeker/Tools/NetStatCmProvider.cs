using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;

namespace MyLab.DockerPeeker.Tools
{
    class NetStatCmProvider : IContainerMetricsProvider
    {
        private readonly IFileContentProvider _fileContentProvider;

        public NetStatCmProvider(IFileContentProvider fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId, string pid)
        {
            var statStr = await _fileContentProvider.ReadNetStat(pid);

            var statParser = StatParser.Create(statStr);

            var netParams = statParser.ExtractNetParams();

            var receive = netParams.Sum(p => p.ReceiveBytes);
            var transmit = netParams.Sum(p => p.TransmitBytes);

            return new []
            {
                new ContainerMetric(receive, ContainerMetricType.NetReceiveMetricType), 
                new ContainerMetric(transmit, ContainerMetricType.NetTransmitMetricType), 
            };
        }
    }
}