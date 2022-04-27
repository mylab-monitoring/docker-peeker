using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;
using MyLab.DockerPeeker.Tools.StatObjectModel;

namespace MyLab.DockerPeeker.Tools.CgroupsV2
{
    class CpuStatContainerMetricsProviderV2 : IContainerMetricsProvider
    {
        private readonly IFileContentProviderV2 _fileContentProvider;

        public CpuStatContainerMetricsProviderV2(IFileContentProviderV2 fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId, string pid)
        {
            var statContent = await _fileContentProvider.ReadCpuStat(containerLongId);

            var stat = KeyValueStat.Parse(statContent);

            if (!stat.TryGetValue("user_usec", out long userValue))
                throw new InvalidOperationException("User CPU stat item 'user_usec' not found");

            if (!stat.TryGetValue("system_usec", out long systemValue))
                throw new InvalidOperationException("System CPU stat item 'system_usec' not found");
            
            return new[]
            {
                new ContainerMetric(userValue/10, ContainerMetricType.CpuJiffiesUserMetricType),
                new ContainerMetric(systemValue/10, ContainerMetricType.CpuJiffiesSystemMetricType),
                new ContainerMetric(userValue, ContainerMetricType.CpuMsUserMetricType),
                new ContainerMetric(systemValue, ContainerMetricType.CpuMsSystemMetricType)
            };
        }
    }
}