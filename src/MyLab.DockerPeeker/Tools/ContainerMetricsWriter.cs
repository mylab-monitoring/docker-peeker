using System.Globalization;
using System.Linq;
using System.Text;
using MyLab.DockerPeeker.Services;

namespace MyLab.DockerPeeker.Tools
{
    class ContainerMetricsWriter
    {
        private readonly ContainerLink _containerLink;
        private readonly ContainerState _containerState;
        private readonly StringBuilder _stringBuilder;

        public ContainerMetricsWriter(
            ContainerLink containerLink,
            ContainerState containerState,
            StringBuilder stringBuilder)
        {
            _containerLink = containerLink;
            _containerState = containerState;
            _stringBuilder = stringBuilder;
        }

        public void Write(ContainerMetric metric)
        {
            var sb = _stringBuilder;

            if(!string.IsNullOrEmpty(metric.Type.Description))
                sb.AppendLine($"# HELP {metric.Type.Description}");
            sb.AppendLine($"# TYPE {metric.Type.Name} {metric.Type.Type}");
            sb.Append($"{metric.Type.Name}{{name=\"{_containerLink.Name}\"");

            if (_containerState != null && _containerState.Labels.Count != 0)
            {
                var keyValues = _containerState.Labels.Select(kv => $"container_label_{NormKey(kv.Key)}=\"{kv.Value}\"");
                var addLabels = string.Join(',', keyValues);
                sb.Append("," + addLabels);
            }

            sb.AppendLine($"}} {metric.Value}");

            string NormKey(string key)
            {
                char[] newString = new char[key.Length];

                for (int i = 0; i < key.Length; i++)
                {
                    newString[i] = char.IsLetterOrDigit(key[i]) ? key[i] : '_';
                }

                return new string(newString);
            }

        }
    }
}