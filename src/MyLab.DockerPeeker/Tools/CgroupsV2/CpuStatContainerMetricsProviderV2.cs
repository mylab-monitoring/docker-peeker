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

            var userValue = stat.GetRequired("user_usec");
            var systemValue = stat.GetRequired("system_usec");
            
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