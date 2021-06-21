using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLab.DockerPeeker.Tools
{
    static class DockerStatProvider
    {
        public static async Task<string> GetStats()
        {
            var res = new StringBuilder();

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "stats --no-stream --no-trunc --format \"{{.Name}}\\t{{.CPUPerc}}\\t{{.MemUsage}}\\t{{.MemPerc}}\\t{{.BlockIO}}\\t{{.NetIO}}\"",
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

            return res.ToString();
        }
    }
}
