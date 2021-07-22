using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;
using MyLab.DockerPeeker.Tools;

namespace MyLab.DockerPeeker.Controllers
{
    [ApiController]
    [Route("metrics")]
    public class MetricsController : ControllerBase
    {
        private readonly ILogger<MetricsController> _logger;
        private readonly MetricsReportBuilder _metricsReportBuilder;
        
        public MetricsController(ILogger<MetricsController> logger,
            MetricsReportBuilder metricsReportBuilder)
        {
            _logger = logger;
            _metricsReportBuilder = metricsReportBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var reportStringBuilder=  new StringBuilder();

            await _metricsReportBuilder.WriteReportAsync(reportStringBuilder);

            return Ok(reportStringBuilder.ToString());
        }
    }
}
