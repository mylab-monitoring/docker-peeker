using System.Collections.Generic;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;

namespace MyLab.DockerPeeker.Tools
{
    class CpuAcctStatCmProvider : IContainerMetricsProvider
    {
        private readonly IFileContentProvider _fileContentProvider;

        public CpuAcctStatCmProvider(IFileContentProvider fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId, string pid)
        {
            var statContent = await _fileContentProvider.ReadCpuStat(containerLongId);
            var parser = StatParser.Create(statContent);

            return new[]
            {
                new ContainerMetric(parser.ExtractKey("user", "User cpu"), ContainerMetricType.CpuUserMetricType),
                new ContainerMetric(parser.ExtractKey("system", "System cpu"), ContainerMetricType.CpuSystemMetricType)
            };
        }
    }
}