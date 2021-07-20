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
        private readonly IContainerStateProvider _containerStateProvider;
        private readonly IContainerMetricsProviderRegistry _containerMetricsProviderRegistry;

        public MetricsReportBuilder(
            IContainerListProvider containerListProvider,
            IContainerStateProvider containerStateProvider,
            IContainerMetricsProviderRegistry containerMetricsProviderRegistry)
        {
            _containerListProvider = containerListProvider;
            _containerStateProvider = containerStateProvider;
            _containerMetricsProviderRegistry = containerMetricsProviderRegistry;
        }

        public async Task WriteReportAsync(StringBuilder reportStringBuilder)
        {
            var containerLinks = await _containerListProvider.ProviderActiveContainersAsync();

            var state = await _containerStateProvider.ProvideAsync(containerLinks);

            var metricsProviders = 
                _containerMetricsProviderRegistry
                    .Provide()
                    .ToArray();

            foreach (var containerLink in containerLinks)
            {
                var containerLabels = state.FirstOrDefault(st => st.Id == containerLink.LongId);

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
