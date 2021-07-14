using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLab.DockerPeeker.Tools
{
    public class ContainerMetricType
    {
        public string Name { get; }
        public string Type { get; }
        public string Description { get; }

        public ContainerMetricType(string name, string type, string description)
        {
            Name = name;
            Type = type;
            Description = description;
        }
    }
}
