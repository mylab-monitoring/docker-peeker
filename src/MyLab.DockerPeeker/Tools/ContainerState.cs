using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyLab.DockerPeeker.Tools
{
    public class ContainerState
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Pid { get; set; }

        public bool IsActive { get; set; }
        public string Status { get; set; }

        public Dictionary<string, string> Labels { get; set; }
        
    }
}