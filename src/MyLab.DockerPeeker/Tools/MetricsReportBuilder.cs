using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyLab.DockerPeeker.Services;
using MyLab.Log;
using MyLab.Log.Dsl;

namespace MyLab.DockerPeeker.Tools
{
    public class MetricsReportBuilder
    {
        private readonly IContainerStateProvider _containerStateProvider;
        private readonly IContainerMetricsProviderRegistry _containerMetricsProviderRegistry;
        private readonly IPeekingReportService _reportService;
        private readonly IDslLogger _log;

        public MetricsReportBuilder(
            IContainerStateProvider containerStateProvider,
            IContainerMetricsProviderRegistry containerMetricsProviderRegistry,
            IPeekingReportService reportService,
            ILogger<MetricsReportBuilder> logger = null)
        {
            _containerStateProvider = containerStateProvider;
            _containerMetricsProviderRegistry = containerMetricsProviderRegistry;
            _reportService = reportService;
            _log = logger?.Dsl();
        }

        public async Task WriteReportAsync(StringBuilder reportStringBuilder)
        {
            ContainerState[] states;
            IContainerMetricsProvider[] metricsProviders;

            try
            {
                states = await _containerStateProvider.ProvideAsync();
                metricsProviders = await _containerMetricsProviderRegistry.ProvideAsync();
            }
            catch (Exception e)
            {
                _log?.Error(e).Write();

                _reportService.Report(new PeekingReport
                {
                    CommonError = e
                });

                return;
            }

            var containerReports = new List<PeekingReportItem>();

            foreach (var containerState in states)
            {
                var writer = new ContainerMetricsWriter(
                    containerState,
                    reportStringBuilder);

                Dictionary<string, ExceptionDto> errors = null; 

                foreach (var metricsProvider in metricsProviders)
                {
                    try
                    {
                        var containerMetrics = await metricsProvider.ProvideAsync(containerState.ContainerId, containerState.Pid);

                        foreach (var containerMetric in containerMetrics)
                        {
                            writer.Write(containerMetric);
                        }
                    }
                    catch (Exception e)
                    {
                        _log?.Error("Container metrics providing error", e)
                            .AndFactIs("container-id", containerState.ContainerId)
                            .AndFactIs("container-name", containerState.Name)
                            .Write();

                        errors ??= new Dictionary<string, ExceptionDto>();
                        errors.Add(metricsProvider.GetType().Name, e);
                    }
                }

                containerReports.Add(new PeekingReportItem
                {
                    State = containerState,
                    Errors = errors
                });
            }

            _reportService.Report(new PeekingReport
            {
                Containers = containerReports.ToArray()
            });
        }
    }
}
