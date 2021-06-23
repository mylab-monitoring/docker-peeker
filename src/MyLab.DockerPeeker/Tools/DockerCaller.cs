﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MyLab.DockerPeeker.Tools
{
    static class DockerCaller
    {
        public const string LabelNamePrefix = "docker_label_";
        public const string ContainerIdSeparator = "<id-separator>";
        public const string StringStartMarker = "<string-start>";

        public static async Task<string[]> GetStats()
        {
            var response =
                await Call(
                    "stats",
                    "--no-stream",
                    "--no-trunc",
                    "--format",
                    StringStartMarker + "{{.ID}}\\t{{.Name}}\\t{{.CPUPerc}}\\t{{.MemUsage}}\\t{{.MemPerc}}\\t{{.BlockIO}}\\t{{.NetIO}}");
            return response
                .Replace("\n", " ")
                .Replace(StringStartMarker, "\n")
                .Split("\n", StringSplitOptions.RemoveEmptyEntries);
        }

        public static async Task<string[]> GetLabels(string[] ids)
        {
            var args = new List<string>
            {
                "inspect",
                "--format",
                StringStartMarker + "{{.ID}}" + ContainerIdSeparator +  "{{ range $k, $v := .Config.Labels }}" + LabelNamePrefix + "{{$k}}={{$v}} {{ end }}'"
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
                throw new InvalidOperationException(err.ToString());

            return res.ToString();
        }
    }
}
