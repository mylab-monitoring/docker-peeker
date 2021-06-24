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
        private readonly IContainerLabelsProvider _containerLabelsProvider;

        public MetricsBuilder(IContainerLabelsProvider containerLabelsProvider)
        {
            _containerLabelsProvider = containerLabelsProvider;
        }

        public async Task Build(StringBuilder sb, DockerStatItem[] stat)
        {
            if (sb == null) throw new ArgumentNullException(nameof(sb));

            var cIds = stat
                .Select(s => s.ContainerId)
                .ToArray();

            var labels = await _containerLabelsProvider.Provide(cIds);

            foreach (var statItem in stat)
            {
                AppendStatItem(sb, statItem, labels.FirstOrDefault(l => l.ContainerId == statItem.ContainerId));
            }
        }

        void AppendStatItem(StringBuilder sb, DockerStatItem item, ContainerLabels containerLabels)
        {
            AppendMetric("container_host_cpu_usage_percentages_total", DockerStatItem.HostCpuUsageDescription, item.HostCpuUsage);
            AppendMetric("container_host_memory_usage_percentages_total", DockerStatItem.HostMemUsageDescription, item.HostMemUsage);
            AppendMetric("container_memory_usage_bytes_total", DockerStatItem.ContainerMemUsageDescription, item.ContainerMemUsage);
            AppendMetric("container_memory_limit_bytes_total", DockerStatItem.ContainerMemLimitDescription, item.ContainerMemLimit);
            AppendMetric("container_block_input_bytes_total", DockerStatItem.BlockInputDescription, item.BlockInput);
            AppendMetric("container_block_output_bytes_total", DockerStatItem.BlockOutputDescription, item.BlockOutput);
            AppendMetric("container_network_input_bytes_total", DockerStatItem.NetInputDescription, item.NetInput);
            AppendMetric("container_network_output_bytes_total", DockerStatItem.NetOutputDescription, item.NetOutput);

            void AppendMetric(string metricName, string description, double value)
            {
                sb.AppendLine($"# HELP {description}");
                sb.AppendLine($"# TYPE {metricName} gauge");
                sb.Append($"{metricName}{{name={item.ContainerName}");

                if (containerLabels != null)
                {
                    var keyValues = containerLabels.Select(kv => $"container_label_{NormKey(kv.Key)}={kv.Value}");
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

                string NormValue(string key)
                {
                    char[] newString = new char[key.Length];

                    for (int i = 0; i < key.Length; i++)
                    {
                        newString[i] = (key[i] != '\"' && key[i] != ',') ? key[i] : '_';
                    }

                    return new string(newString);
                }
            }
        }
    }
}
