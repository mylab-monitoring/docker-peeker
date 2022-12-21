using System.Collections.Generic;
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
            sb.Append($"{metric.Type.Name}{{container_name=\"{_containerLink.Name}\"");

            var labels = new Dictionary<string,string>();

            if (metric.Type.Labels != null)
            {
                foreach (var typeLabel in metric.Type.Labels)
                {
                    labels.Add(typeLabel.Key, typeLabel.Value);
                }
            }

            if (_containerState?.Labels != null)
            {
                foreach (var stateLabel in _containerState.Labels)
                {
                    var key = "container_label_" + NormKey(stateLabel.Key);
                    if (labels.ContainsKey(key))
                    {
                        labels[key] = stateLabel.Value;
                    }
                    else
                    {
                        labels.Add(key, stateLabel.Value);
                    }
                }
            }


            if (labels.Count != 0)
            {
                var keyValues = labels.Select(kv => $"{kv.Key}=\"{StringEscape.Escape(kv.Value)}\"");
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