using System.Collections.Generic;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;

namespace MyLab.DockerPeeker.Tools
{
    class CpuAcctStatCmProvider : IContainerMetricsProvider
    {
        private readonly IFileContentProvider _fileContentProvider;
        private readonly ContainerMetricType _cpuUserMetricType;
        readonly ContainerMetricType _cpuSystemMetricType;

        public CpuAcctStatCmProvider(IFileContentProvider fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;

            var cpuMetricType = new ContainerMetricType
            {
                Name = "container_cpu_jiffies_total",
                Type = "counter"
            };

            _cpuUserMetricType = cpuMetricType.AddLabel("mode", "user", "Time is the amount of time a process has direct control of the CPU, executing process code");
            _cpuSystemMetricType = cpuMetricType.AddLabel("mode", "system", "Time is the time the kernel is executing system calls on behalf of the process");
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