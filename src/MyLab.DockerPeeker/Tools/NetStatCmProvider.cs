using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;

namespace MyLab.DockerPeeker.Tools
{
    class NetStatCmProvider : IContainerMetricsProvider
    {
        private readonly IFileContentProvider _fileContentProvider;

        private readonly ContainerMetricType _netReceiveMetricType;

        private readonly ContainerMetricType _netTransmitMetricType;

        public NetStatCmProvider(IFileContentProvider fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;

            var netReceiveMetricType = new ContainerMetricType
            {
                Name = "container_net_bytes_total",
                Type = "counter"
            };

            _netReceiveMetricType = netReceiveMetricType.AddLabel("direction", "receive", "Report total received bytes");
            _netTransmitMetricType = netReceiveMetricType.AddLabel("direction", "transmit", "Report total transmitted bytes");
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
                new ContainerMetric(receive, _netReceiveMetricType), 
                new ContainerMetric(transmit, _netTransmitMetricType), 
            };
        }
    }
}