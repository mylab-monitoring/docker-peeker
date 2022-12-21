using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyLab.DockerPeeker.Tools
{
    public class ContainerState
    {
        public string Id { get; }
        public string Pid { get; }

        public ReadOnlyDictionary<string, string> Labels { get; }

        public ContainerState(string id, string pid, IDictionary<string,string> labels)
        {
            Id = id;
            Pid = pid;
            Labels = new ReadOnlyDictionary<string, string>(labels);
        }

        public static ContainerState Parse(string dockerOutput)
        {
            if (string.IsNullOrWhiteSpace(dockerOutput))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dockerOutput));

            var normOutput = dockerOutput.Trim();
            
            int indexOfDockerIdSplitter = normOutput.IndexOf(DockerCaller.ContainerIdSeparator, StringComparison.InvariantCulture);
            if (indexOfDockerIdSplitter == -1)
                throw new FormatException("Cant find container id separator: " + normOutput);
            string containerId = normOutput.Remove(indexOfDockerIdSplitter);

            int indexOfDockerPIdSplitter = normOutput.IndexOf(DockerCaller.ContainerPidSeparator, StringComparison.InvariantCulture);
            if (indexOfDockerPIdSplitter == -1)
                throw new FormatException("Cant find container pid separator: " + normOutput);
            string containerPid = normOutput.Substring(indexOfDockerIdSplitter+ DockerCaller.ContainerIdSeparator.Length, indexOfDockerPIdSplitter- (indexOfDockerIdSplitter + DockerCaller.ContainerIdSeparator.Length));
            
            string pairsStr = normOutput.Substring(indexOfDockerPIdSplitter + DockerCaller.ContainerPidSeparator.Length).Trim();

            var labelPairs = new Dictionary<string, string>();

            if (pairsStr.Length != 0)
            {
                var pairs = pairsStr
                    .Replace(DockerCaller.LabelNamePrefix, "\n")
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries);

                foreach (var pair in pairs)
                {
                    var separatorIndex = pair.IndexOf('=');

                    if (separatorIndex < 0) 
                        throw new FormatException("Separator '=' not found");

                    var val = separatorIndex == pair.Length-1
                        ? string.Empty
                        : pair.Substring(separatorIndex+1);

                    labelPairs.Add(pair.Remove(separatorIndex).Trim(), val.Trim());
                }
            }

            return new ContainerState(containerId, containerPid, labelPairs);
        }
    }
}