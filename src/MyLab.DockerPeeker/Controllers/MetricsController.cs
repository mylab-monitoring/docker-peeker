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
        private readonly IDockerStatProvider _dockerStatProvider;
        private readonly MetricsBuilder _metricBuilder;

        public MetricsController(ILogger<MetricsController> logger, 
            IDockerStatProvider dockerStatProvider,
            IContainerLabelsProvider containerLabelsProvider)
        {
            _logger = logger;
            _dockerStatProvider = dockerStatProvider;
            _metricBuilder = new MetricsBuilder(containerLabelsProvider);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var stat = await _dockerStatProvider.Provide();

            var sb= new StringBuilder();

            await _metricBuilder.Build(sb, stat);

            return Ok(sb.ToString());
        }
    }
}
