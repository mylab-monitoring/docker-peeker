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
            "container_cpu_user_jiffies",
            "counter",
            "Time is the amount of time a process has direct control of the CPU, executing process code");

        readonly ContainerMetricType _cpuSystemMetricType = new ContainerMetricType(
            "container_cpu_system_jiffies",
            "counter",
            "Time is the time the kernel is executing system calls on behalf of the process");

        public CpuAcctStatCmProvider(IFileContentProvider fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId)
        {
            var statContent = await _fileContentProvider.ReadCpuAcctStat(containerLongId);
            var parser = StatParser.Create(statContent);

            return new[]
            {
                new ContainerMetric(parser.Extract("user", "User cpu"), _cpuUserMetricType),
                new ContainerMetric(parser.Extract("system", "System cpu"), _cpuSystemMetricType)
            };
        }
    }
}