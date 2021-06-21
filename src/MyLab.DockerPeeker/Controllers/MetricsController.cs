using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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
            var res = new StringBuilder();

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "docker", 
                Arguments = "ps -a",
                RedirectStandardOutput = true
            };
            Process proc = new Process
            {
                StartInfo = startInfo
            };
            proc.Start();

            while (!proc.StandardOutput.EndOfStream)
            {
                res.AppendLine(proc.StandardOutput.ReadLine());
            }

            await proc.WaitForExitAsync();

            return Ok(res.ToString());
        }
    }
}
