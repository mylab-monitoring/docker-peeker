using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyLab.DockerPeeker.Tools
{
    public class ContainerState
    {
        public string Id { get; }
        public string Pid { get; }

        public ReadOnlyDictionary<string, string> Labels { get; }

        public ContainerState(string id, string pid, IDictionary<string, string> labels)
        {
            Id = id;
            Pid = pid;
            Labels = new ReadOnlyDictionary<string, string>(labels);
        }
    }
}