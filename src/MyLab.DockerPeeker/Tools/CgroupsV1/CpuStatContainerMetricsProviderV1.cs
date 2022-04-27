using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;
using MyLab.DockerPeeker.Tools.StatObjectModel;

namespace MyLab.DockerPeeker.Tools.CgroupsV1
{
    class CpuStatContainerMetricsProviderV1 : IContainerMetricsProvider
    {
        private readonly IFileContentProviderV1 _fileContentProvider;

        public CpuStatContainerMetricsProviderV1(IFileContentProviderV1 fileContentProvider)
        {
            _fileContentProvider = fileContentProvider;
        }

        public async Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId, string pid)
        {
            var statContent = await _fileContentProvider.ReadCpuStat(containerLongId);

            var stat = KeyValueStat.Parse(statContent);

            if(!stat.TryGetValue("user", out long userValue))
                throw new InvalidOperationException("User CPU stat item 'user' not found");

            if (!stat.TryGetValue("system", out long systemValue))
                throw new InvalidOperationException("System CPU stat item 'system' not found");

            return new[]
            {
                new ContainerMetric(userValue, ContainerMetricType.CpuJiffiesUserMetricType),
                new ContainerMetric(systemValue, ContainerMetricType.CpuJiffiesSystemMetricType),
                new ContainerMetric(userValue*10, ContainerMetricType.CpuMsUserMetricType),
                new ContainerMetric(systemValue*10, ContainerMetricType.CpuMsSystemMetricType)
            };
        }
    }
}