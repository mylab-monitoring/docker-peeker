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

            var states = await _containerStateProvider.ProvideAsync(containerLinks);

            var metricsProviders = 
                _containerMetricsProviderRegistry
                    .Provide()
                    .ToArray();

            foreach (var containerLink in containerLinks)
            {
                var containerState = states.FirstOrDefault(st => st.Id == containerLink.LongId);

                var writer = new ContainerMetricsWriter(
                    containerLink, 
                    containerState,
                    reportStringBuilder);

                foreach (var metricsProvider in metricsProviders)
                {
                    var containerMetrics = await metricsProvider.ProvideAsync(containerLink.LongId, containerState?.Pid);

                    foreach (var containerMetric in containerMetrics)
                    {
                        writer.Write(containerMetric);
                    }
                }
            }

        }
    }
}
