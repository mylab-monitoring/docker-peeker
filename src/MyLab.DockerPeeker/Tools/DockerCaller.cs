using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MyLab.DockerPeeker.Tools
{
    static class DockerCaller
    {
        public const string LabelNamePrefix = "docker_label_";
        public const string ContainerIdSeparator = "<id-separator>";

        public static async Task<string[]> GetStats()
        {
            var response =
                await Call("stats --no-stream --no-trunc --format \"{{.ID}}\\t{{.Name}}\\t{{.CPUPerc}}\\t{{.MemUsage}}\\t{{.MemPerc}}\\t{{.BlockIO}}\\t{{.NetIO}}\"");
            return response.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        }

        public static async Task<string[]> GetLabels(string[] ids)
        {
            var response = await Call("inspect --format '{{.ID}}" + ContainerIdSeparator +  "{{ range $k, $v := .Config.Labels }}" + LabelNamePrefix + "{{$k}}={{$v}} {{ end }}' " + string.Join(" ", ids));

            return response.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        }

        static async Task<string> Call(string command)
        {
            var res = new StringBuilder();
            var err = new StringBuilder();

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = command,
                RedirectStandardOutput = true,
                RedirectStandardError = true
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

            while (!proc.StandardError.EndOfStream)
            {
                err.AppendLine(proc.StandardError.ReadLine());
            }

            await proc.WaitForExitAsync();

            if(proc.ExitCode != 0)
                throw new InvalidOperationException(err.ToString());

            return res.ToString();
        }
    }
}
