using System.Globalization;
using System.Linq;
using System.Text;
using MyLab.DockerPeeker.Services;

namespace MyLab.DockerPeeker.Tools
{
    class ContainerMetricsWriter
    {
        private readonly ContainerLink _containerLink;
        private readonly ContainerLabels _containerLabels;
        private readonly StringBuilder _stringBuilder;

        public ContainerMetricsWriter(
            ContainerLink containerLink, 
            ContainerLabels containerLabels,
            StringBuilder stringBuilder)
        {
            _containerLink = containerLink;
            _containerLabels = containerLabels;
            _stringBuilder = stringBuilder;
        }

        public void Write(ContainerMetric metric)
        {
            var sb = _stringBuilder;

            if(!string.IsNullOrEmpty(metric.Type.Description))
                sb.AppendLine($"# HELP {metric.Type.Description}");
            sb.AppendLine($"# TYPE {metric.Type.Name} {metric.Type.Type}");
            sb.Append($"{metric.Type.Name}{{name=\"{_containerLink.Name}\"");

            if (_containerLabels != null && _containerLabels.Count != 0)
            {
                var keyValues = _containerLabels.Select(kv => $"container_label_{NormKey(kv.Key)}=\"{kv.Value}\"");
                var addLabels = string.Join(',', keyValues);
                sb.Append("," + addLabels);
            }

            sb.AppendLine($"}} {metric.Value.ToString("F2", CultureInfo.InvariantCulture)}");

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