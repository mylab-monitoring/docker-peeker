using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyLab.DockerPeeker.Tools
{
    public class ContainerState
    {
        public string Name { get; set; }
        public string ContainerId { get; set; }
        public string Pid { get; set; }
        public Dictionary<string, string> Labels { get; set; }
        
    }
}