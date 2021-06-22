using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MyLab.DockerPeeker.Tools
{
    public class DockerStatItem
    {
        public const string HostCpuUsageDescription = "The percentage of the host’s CPU the container is using";
        public const string HostMemUsageDescription = "The percentage of the host’s memory the container is using";
        public const string ContainerMemUsageDescription = "The total memory the container is using";
        public const string ContainerMemLimitDescription = "The total amount of memory it is allowed to use";
        public const string BlockInputDescription = "The amount of data the container has read from block devices on the host";
        public const string BlockOutputDescription = "The amount of data the container has written to block devices on the host";
        public const string NetInputDescription = "The amount of data the container received over its network interface";
        public const string NetOutputDescription = "The amount of data the container has sent over its network interface";

        /// <summary>
        /// The ID of the container
        /// </summary>
        public string ContainerId { get; set; }

        /// <summary>
        /// The name of the container
        /// </summary>
        public string ContainerName { get; set; }
        /// <summary>
        /// <see cref="HostCpuUsageDescription"/>
        /// </summary>
        public double HostCpuUsage { get; set; }
        /// <summary>
        /// <see cref="ContainerMemUsageDescription"/>
        /// </summary>
        public double HostMemUsage { get; set; }
        /// <summary>
        /// <see cref="ContainerMemUsageDescription"/>
        /// </summary>
        public double ContainerMemUsage { get; set; }
        /// <summary>
        /// <see cref="ContainerMemLimitDescription"/>
        /// </summary>
        public double ContainerMemLimit { get; set; }
        /// <summary>
        /// <see cref="BlockInputDescription"/>
        /// </summary>
        public double BlockInput { get; set; }
        /// <summary>
        /// <see cref="BlockOutputDescription"/>
        /// </summary>
        public double BlockOutput { get; set; }
        /// <summary>
        /// <see cref="NetInputDescription"/>
        /// </summary>
        public double NetInput { get; set; }
        /// <summary>
        /// <see cref="NetOutputDescription"/>
        /// </summary>
        public double NetOutput { get; set; }

        public void ToMetrics(StringBuilder sb)
        {
            if (sb == null) throw new ArgumentNullException(nameof(sb));

            AppendMetric(sb, "container_host_cpu_usage_percentages_total", HostCpuUsageDescription, HostCpuUsage);
            AppendMetric(sb, "container_host_memory_usage_percentages_total", HostMemUsageDescription,  HostMemUsage);
            AppendMetric(sb, "container_memory_usage_bytes_total", ContainerMemUsageDescription, ContainerMemUsage);
            AppendMetric(sb, "container_memory_limit_bytes_total", ContainerMemLimitDescription, ContainerMemLimit);
            AppendMetric(sb, "container_block_input_bytes_total", BlockInputDescription, BlockInput);
            AppendMetric(sb, "container_block_output_bytes_total", BlockOutputDescription, BlockOutput);
            AppendMetric(sb, "container_network_input_bytes_total", NetInputDescription, NetInput);
            AppendMetric(sb, "container_network_output_bytes_total", NetOutputDescription, NetOutput);
        }

        void AppendMetric(StringBuilder sb, string metricName, string description, double value)
        {
            sb.AppendLine($"# HELP {description}");
            sb.AppendLine($"# TYPE {metricName} guage");
            sb.AppendLine($"metricName{{name={ContainerName}}} {value:F2}");
        }

        public static DockerStatItem Parse(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(str));

            var items = str.Split('\t');

            if(items.Length != 7)
                throw new FormatException("Wrong count or parameters");

            ParsePair(items[3], "MemUsage", out var cMem, out var cLim);
            ParsePair(items[5], "BlockIO", out var blockIn, out var blockOut);
            ParsePair(items[6], "NetIO", out var netIn, out var netOut);

            return new DockerStatItem
            {
                ContainerId = items[0],
                ContainerName = items[1],
                HostCpuUsage = ParsePercentage(items[2], "CPUPerc"),
                HostMemUsage = ParsePercentage(items[4], "MemPerc"),
                ContainerMemUsage = cMem,
                ContainerMemLimit = cLim,
                BlockInput = blockIn,
                BlockOutput = blockOut,
                NetInput = netIn,
                NetOutput = netOut
            };
        }

        private static double ParsePercentage(string item, string name)
        {
            if(!item.EndsWith("%"))
                throw new FormatException($"Parameter '{name}' has wrong format: percentage not found");
            var digitStr = item.TrimEnd('%');

            if(!double.TryParse(digitStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var res))
                throw new FormatException($"Parameter '{name}' has wrong format: cant parse 'double' from string '{digitStr}'");

            return res;
        }

        private static void ParsePair(string item, string name, out double v1, out double v2)
        {
            var pairItems = item
                .Split('/')
                .Select(s => s.Trim())
                .ToArray();

            if(pairItems.Length != 2)
                throw new FormatException($"Parameter '{name}' has wrong format: is not a value pair");

            v1 = ParseVolume(pairItems[0], name);
            v2 = ParseVolume(pairItems[1], name);
        }

        private static double ParseVolume(string str, string name)
        {
            string foundKey = VolumesMap
                .Keys
                .OrderByDescending(k => k.Length)
                .FirstOrDefault(str.EndsWith);

            if(foundKey == null)
                throw new FormatException($"Parameter '{name}' has wrong format: is not volume value '{str}'");
            string digitStr = str.Remove(str.Length - foundKey.Length);

            if (!double.TryParse(digitStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var res))
                throw new FormatException($"Parameter '{name}' has wrong format: cant parse 'double' from second value '{digitStr}'");

            return res * VolumesMap[foundKey];
        }

        static readonly IDictionary<string, int> VolumesMap = new Dictionary<string, int>
        {
            { "B", 1 },
            { "kB", 1024 },
            { "MiB", 1024*1024 },
            { "GiB", 1024*1024*1024 },
        };
    }
}