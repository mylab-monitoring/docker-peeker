using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;

namespace MyLab.DockerPeeker.Tools
{
    public class MetricsReportBuilder
    {
        private readonly IContainerListProvider _containerListProvider;
        private readonly IContainerLabelsProvider _containerLabelsProvider;
        private readonly IContainerMetricsProviderRegistry _containerMetricsProviderRegistry;

        public MetricsReportBuilder(
            IContainerListProvider containerListProvider,
            IContainerLabelsProvider containerLabelsProvider,
            IContainerMetricsProviderRegistry containerMetricsProviderRegistry)
        {
            _containerListProvider = containerListProvider;
            _containerLabelsProvider = containerLabelsProvider;
            _containerMetricsProviderRegistry = containerMetricsProviderRegistry;
        }

        public async Task WriteReportAsync(StringBuilder reportStringBuilder)
        {
            var containerLinks = await _containerListProvider.ProviderActiveContainersAsync();
            var containerIds = containerLinks
                .Select(l => l.LongId)
                .ToArray();

            var labels = await _containerLabelsProvider.ProvideAsync(containerIds);

            var metricsProviders = 
                _containerMetricsProviderRegistry
                    .Provide()
                    .ToArray();

            foreach (var containerLink in containerLinks)
            {
                var containerLabels = labels.FirstOrDefault(lc => lc.ContainerId == containerLink.LongId);

                var writer = new ContainerMetricsWriter(
                    containerLink, 
                    containerLabels,
                    reportStringBuilder);

                foreach (var metricsProvider in metricsProviders)
                {
                    var containerMetrics = await metricsProvider.ProvideAsync(containerLink.LongId);

                    foreach (var containerMetric in containerMetrics)
                    {
                        writer.Write(containerMetric);
                    }
                }
            }

        }
    }
}
