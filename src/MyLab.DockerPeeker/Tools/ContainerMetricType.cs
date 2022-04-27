using System.Collections.Generic;

namespace MyLab.DockerPeeker.Tools
{
    public partial class ContainerMetricType
    {
        readonly Dictionary<string, string> _labels;

        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public IDictionary<string, string> Labels => _labels;

        public ContainerMetricType()
            : this(new Dictionary<string, string>())
        {
        }

        public ContainerMetricType(ContainerMetricType containerMetricType)
            : this(containerMetricType._labels)
        {
            Name = containerMetricType.Name;
            Type = containerMetricType.Type;
            Description = containerMetricType.Description;
        }

        ContainerMetricType(IDictionary<string, string> labels)
        {
            _labels = new Dictionary<string, string>(labels);
        }

        public ContainerMetricType AddLabel(string name, string value, string newDescription = null)
        {
            var newType = new ContainerMetricType(this);

            if (newDescription != null)
                newType.Description = newDescription;

            newType._labels.Add(name, value);

            return newType;
        }
    }
}
