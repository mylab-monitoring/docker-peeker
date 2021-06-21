using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Tools;

namespace MyLab.DockerPeeker.Controllers
{
    [ApiController]
    [Route("metrics")]
    public class MetricsController : ControllerBase
    {
        private readonly ILogger<MetricsController> _logger;

        public MetricsController(ILogger<MetricsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var strStat = await DockerStatProvider.GetStats();
            var stat = DockerStatParser.Parse(strStat);

            var sb= new StringBuilder();

            foreach (var dockerStatItem in stat)
            {
                dockerStatItem.ToMetrics(sb);
            }

            return Ok(sb.ToString());
        }
    }
}
