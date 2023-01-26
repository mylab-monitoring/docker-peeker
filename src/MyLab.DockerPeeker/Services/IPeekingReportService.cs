using System;
using System.Collections.Generic;
using MyLab.DockerPeeker.Tools;

namespace MyLab.DockerPeeker.Services
{
    public interface IPeekingReportService
    {
        PeekingReport GetReport();
        void Report(PeekingReport report);
    }

    class PeekingReportService : IPeekingReportService
    {
        private PeekingReport _report;

        public PeekingReport GetReport()
        {
            return _report;
        }

        public void Report(PeekingReport report)
        {
            _report = report;
        }
    }

    public class PeekingReport
    {
        public Exception CommonError { get; set; }

        public PeekingReportItem[] Containers { get; set; }
    }

    public class PeekingReportItem
    {
        public ContainerState State { get; set; }

        public Dictionary<string, Exception> Errors { get; set; }
    }
}
