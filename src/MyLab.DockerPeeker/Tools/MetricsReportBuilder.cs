using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyLab.DockerPeeker.Services;
using MyLab.Log.Dsl;

namespace MyLab.DockerPeeker.Tools
{
    public class MetricsReportBuilder
    {
        private readonly IContainerListProvider _containerListProvider;
        private readonly IContainerStateProvider _containerStateProvider;
        private readonly IContainerMetricsProviderRegistry _containerMetricsProviderRegistry;
        private readonly IDslLogger _log;

        

        public MetricsReportBuilder(
            IContainerListProvider containerListProvider,
            IContainerStateProvider containerStateProvider,
            IContainerMetricsProviderRegistry containerMetricsProviderRegistry,
            ILogger<MetricsReportBuilder> logger = null)
        {
            _containerListProvider = containerListProvider;
            _containerStateProvider = containerStateProvider;
            _containerMetricsProviderRegistry = containerMetricsProviderRegistry;
            _log = logger?.Dsl();
        }

        public async Task WriteReportAsync(StringBuilder reportStringBuilder)
        {
            var containerLinks = await _containerListProvider.ProviderActiveContainersAsync();

            var states = await _containerStateProvider.ProvideAsync(containerLinks);

            var metricsProviders = await _containerMetricsProviderRegistry.ProvideAsync();

            foreach (var containerLink in containerLinks)
            {
                var containerState = states.FirstOrDefault(st => st.Id == containerLink.LongId);

                var writer = new ContainerMetricsWriter(
                    containerLink, 
                    containerState,
                    reportStringBuilder);

                foreach (var metricsProvider in metricsProviders)
                {
                    try
                    {
                        var containerMetrics = await metricsProvider.ProvideAsync(containerLink.LongId, containerState?.Pid);

                        foreach (var containerMetric in containerMetrics)
                        {
                            writer.Write(containerMetric);
                        }
                    }
                    catch (Exception e)
                    {
                        _log?.Error("Container metrics providing error", e)
                            .AndFactIs("container-link", containerLink)
                            .Write();
                    }
                }
            }

        }
    }
}
