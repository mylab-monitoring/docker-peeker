using System.Globalization;
using System.Linq;
using MyLab.Logging;

namespace MyLab.DockerPeeker.Tools
{
    class StatParser
    {
        private readonly string[] _lines;

        public StatParser(string[] lines)
        {
            _lines = lines;
        }

        public static StatParser Create(string fileContent)
        {
            return new StatParser(fileContent.Split('\n').Select(s => s.Trim()).ToArray());
        }

        public long Extract(string key, string name)
        {
            var value = _lines.FirstOrDefault(l => l.StartsWith(key + " "));
            if (value == null)
                throw new PseudoFileFormatException(name + " not found");
            var stringValue = value.Substring(key.Length + 1);
            if (!long.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var valValue))
                throw new PseudoFileFormatException(name + " has wrong format")
                    .AndFactIs("value", stringValue);

            return valValue;
        }
    }
}