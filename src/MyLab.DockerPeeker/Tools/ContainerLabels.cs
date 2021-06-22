using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyLab.DockerPeeker.Tools
{
    public class ContainerLabels : ReadOnlyDictionary<string, string>
    {
        public string ContainerId { get; }

        public ContainerLabels(string containerId, IDictionary<string, string> labels)
            :base(labels)
        {
            ContainerId = containerId;
        }

        public static ContainerLabels Parse(string dockerContainerPs)
        {
            if (string.IsNullOrWhiteSpace(dockerContainerPs))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dockerContainerPs));

            int indexOfDockerIdSplitter = dockerContainerPs.IndexOf(DockerCaller.ContainerIdSeparator);
            if(indexOfDockerIdSplitter == -1)
                throw new FormatException("Cant find container id separator: " + dockerContainerPs);

            string containerId = dockerContainerPs.Remove(indexOfDockerIdSplitter);
            string pairsStr = dockerContainerPs.Substring(indexOfDockerIdSplitter + DockerCaller.ContainerIdSeparator.Length).Trim();

            var labelPairs = new Dictionary<string,string>();

            if (pairsStr.Length != 0)
            {
                var pairs = pairsStr
                    .Replace(DockerCaller.LabelNamePrefix, "\n")
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries);

                foreach (var pair in pairs)
                {
                    var pairItems = pair.Split('=');

                    if(pairItems.Length > 2)
                        throw new FormatException($"Too many pair items: '{pair}'");

                    var pairValue = pairItems.Length == 2
                        ? pairItems[1]
                        : string.Empty;

                    try
                    {
                        labelPairs.Add(pairItems[0].Trim(), pairValue.Trim());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    
                }
            }

            return new ContainerLabels(containerId, labelPairs);
        }
    }
}
