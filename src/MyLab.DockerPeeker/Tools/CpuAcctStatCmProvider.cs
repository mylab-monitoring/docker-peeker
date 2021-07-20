using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;
using MyLab.Logging;

namespace MyLab.DockerPeeker.Tools
{
    class CpuAcctStatCmProvider : IContainerMetricsProvider
    {
        private readonly IFileContentProvider _fileContentProvider;

        readonly ContainerMetricType _cpuUserMetricType = new ContainerMetricType(
            "container_cpu_user_jiffies_total",
            "counter",
            "Time is the amount of time a process has direct control of the CPU, executing process code");

        readonly ContainerMetricType _cpuSystemMetricType = new ContainerMetricType(
            "container_cpu_system_jiffies_total",
            "counter",
            "Time is the time the kernel is executing system calls on behalf of the process");

        public CpuAcctStatCmProvider(IFileContentProvider fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId)
        {
            var statContent = await _fileContentProvider.ReadCpuAcct(containerLongId);
            var stat = CpuAcctStat.Parse(statContent);

            return new[]
            {
                new ContainerMetric(stat.User, _cpuUserMetricType),
                new ContainerMetric(stat.System, _cpuSystemMetricType)
            };
        }
    }

    class CpuAcctStat
    {
        public long User { get; set; }

        public long System { get; set; }

        public static CpuAcctStat Parse(string stringContent)
        {
            var lines = stringContent.Split('\n');

            var user = lines.FirstOrDefault(l => l.StartsWith("user "));
            if (user == null)
                throw new PseudoFileFormatException("User cpu not found");
            var userStringVal = user.Substring(5).Trim();
            if (!long.TryParse(userStringVal, NumberStyles.Any, CultureInfo.InvariantCulture, out var userValue))
                throw new PseudoFileFormatException("User cpu has wrong format")
                    .AndFactIs("value", userStringVal);


            var system = lines.FirstOrDefault(l => l.StartsWith("system "));
            if (system == null)
                throw new PseudoFileFormatException("System cpu not found");
            var systemStringVal = system.Substring(7).Trim();
            if (!long.TryParse(systemStringVal, NumberStyles.Any, CultureInfo.InvariantCulture, out var systemValue))
                throw new PseudoFileFormatException("System cpu has wrong format")
                    .AndFactIs("value", systemStringVal);

            return new CpuAcctStat
            {
                User = userValue,
                System = systemValue
            };

        }
    }
}