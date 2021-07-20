using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyLab.Log;
using MyLab.Log.Dsl;

namespace MyLab.DockerPeeker.Tools
{
    class DockerCaller
    {
        public const string LabelNamePrefix = "docker_label_";
        public const string ContainerIdSeparator = "<id-separator>";
        public const string ContainerPidSeparator = "<pid-separator>";
        public const string StringStartMarker = "<string-start>";

        public IDslLogger Logger { get; set; }

        public async Task<string[]> GetActiveContainersAsync()
        {
            var response =
                await Call(
                    "ps",
                    "--no-trunc",
                    "--format",
                    "{{.ID}}\t{{.Names}}\t{{.CreatedAt}}");
            return response.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        }

        public async Task<string[]> GetStates(string[] ids)
        {
            var args = new List<string>
            {
                "inspect",
                "--format",
                StringStartMarker + "{{.ID}}" + ContainerIdSeparator + "{{ .State.Pid }}" + ContainerPidSeparator + "{{ range $k, $v := .Config.Labels }}" + LabelNamePrefix + "{{$k}}={{$v}} {{ end }}"
            };

            args.AddRange(ids);

            var response = await Call(args.ToArray());

            return response
                .Replace("\n", " ")
                .Replace(StringStartMarker, "\n")
                .Split("\n", StringSplitOptions.RemoveEmptyEntries);
        }

        static async Task<string> Call(params string[] args)
        {
            var res = new StringBuilder();
            var err = new StringBuilder();

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "docker",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            foreach (var arg in args)
            {
                startInfo.ArgumentList.Add(arg);
            }

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
                throw new InvalidOperationException(err.ToString())
                    .AndFactIs("proc-fn", startInfo.FileName)
                    .AndFactIs("proc-params", string.Join(" ", startInfo.ArgumentList.Select(a => $"\"{a}\"")));

            return res.ToString();
        }
    }
}
