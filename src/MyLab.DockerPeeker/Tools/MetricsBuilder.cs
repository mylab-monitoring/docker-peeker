using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;

namespace MyLab.DockerPeeker.Tools
{
    public class MetricsBuilder
    {
        private readonly IContainerStateProvider _containerStateProvider;

        public MetricsBuilder(IContainerStateProvider containerStateProvider)
        {
            _containerStateProvider = containerStateProvider;
        }

        public async Task Build(StringBuilder sb, DockerStatItem[] stat)
        {
            if (sb == null) throw new ArgumentNullException(nameof(sb));

            var cIds = stat
                .Select(s => s.ContainerId)
                .ToArray();

            var states = await _containerStateProvider.ProvideAsync(cIds);

            foreach (var statItem in stat)
            {
                AppendStatItem(sb, statItem, states.FirstOrDefault(state => state.Id == statItem.ContainerId));
            }
        }

        void AppendStatItem(StringBuilder sb, DockerStatItem item, ContainerState containerState)
        {
            AppendMetric("container_host_cpu_usage_percentages_total", DockerStatItem.HostCpuUsageDescription, item.HostCpuUsage);
            AppendMetric("container_host_memory_usage_percentages_total", DockerStatItem.HostMemUsageDescription, item.HostMemUsage);
            AppendMetric("container_memory_usage_bytes_total", DockerStatItem.ContainerMemUsageDescription, item.ContainerMemUsage);
            AppendMetric("container_memory_limit_bytes_total", DockerStatItem.ContainerMemLimitDescription, item.ContainerMemLimit);
            AppendMetric("container_block_rx_bytes_total", DockerStatItem.BlockRxDescription, item.BlockRx);
            AppendMetric("container_block_tx_bytes_total", DockerStatItem.BlockTxDescription, item.BlockTx);
            AppendMetric("container_network_rx_bytes_total", DockerStatItem.NetRxDescription, item.NetRx);
            AppendMetric("container_network_tx_bytes_total", DockerStatItem.NetTxDescription, item.NetTx);

            void AppendMetric(string metricName, string description, double value)
            {
                sb.AppendLine($"# HELP {description}");
                sb.AppendLine($"# TYPE {metricName} gauge");
                sb.Append($"{metricName}{{name=\"{item.ContainerName}\"");

                if (containerState != null)
                {
                    var keyValues = containerState.Labels.Select(kv => $"container_label_{NormKey(kv.Key)}=\"{kv.Value}\"");
                    var addLabels = string.Join(',', keyValues);
                    sb.Append("," + addLabels);
                }

                sb.AppendLine($"}} {value.ToString("F2", CultureInfo.InvariantCulture)}");

                string NormKey(string key)
                {
                    char[] newString = new char[key.Length];

                    for (int i = 0; i < key.Length; i++)
                    {
                        newString[i] = char.IsLetterOrDigit(key[i]) ? key[i] : '_';
                    }

                    return new string(newString);
                }
            }
        }
    }
}
