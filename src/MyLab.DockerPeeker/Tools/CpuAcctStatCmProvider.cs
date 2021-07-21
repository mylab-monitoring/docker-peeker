using System.Collections.Generic;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;

namespace MyLab.DockerPeeker.Tools
{
    class CpuAcctStatCmProvider : IContainerMetricsProvider
    {
        private readonly IFileContentProvider _fileContentProvider;

        readonly ContainerMetricType _cpuUserMetricType = new ContainerMetricType
        {
            Name = "container_cpu_user_jiffies_total",
            Type = "counter",
            Description = "Time is the amount of time a process has direct control of the CPU, executing process code"
        };

        readonly ContainerMetricType _cpuSystemMetricType = new ContainerMetricType
        {
            Name = "container_cpu_system_jiffies_total",
            Type = "counter",
            Description = "Time is the time the kernel is executing system calls on behalf of the process"
        };

        public CpuAcctStatCmProvider(IFileContentProvider fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId, string pid)
        {
            var statContent = await _fileContentProvider.ReadCpuAcctStat(containerLongId);
            var parser = StatParser.Create(statContent);

            return new[]
            {
                new ContainerMetric(parser.ExtractKey("user", "User cpu"), _cpuUserMetricType),
                new ContainerMetric(parser.ExtractKey("system", "System cpu"), _cpuSystemMetricType)
            };
        }
    }
}