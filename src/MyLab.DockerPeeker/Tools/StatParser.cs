using System.Collections.Generic;
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

        public long ExtractKey(string key, string name)
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

        public IdentifiableParam[] ExtractIdentifiable(string id, string name)
        {
            var values = _lines
                .Where(l => l.StartsWith(id + " "))
                .ToArray();

            var list =new List<IdentifiableParam>();
            
            foreach (var val in values)
            {
                var items = val.Split(' ');

                if(items.Length != 3)
                    throw new PseudoFileFormatException(name + " has wrong parts count")
                        .AndFactIs("value", values);

                if (!long.TryParse(items[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var valValue))
                    throw new PseudoFileFormatException(name + " has wrong format")
                        .AndFactIs("value", items[2]);

                list.Add(new IdentifiableParam
                {
                    Id = items[0],
                    Key = items[1],
                    Value = valValue
                });
            }

            return list.ToArray();
        }

        public IdentifiableParam[] ExtractIdentifiable()
        {
            var values = _lines
                .Select(l => l.Split(' '))
                .Where(ls => ls.Length == 3)
                .ToArray();

            var list = new List<IdentifiableParam>();

            foreach (var items in values)
            {
                if (!long.TryParse(items[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var valValue))
                    throw new PseudoFileFormatException("Identifiable value has wrong format")
                        .AndFactIs("id", items[0])
                        .AndFactIs("key", items[1])
                        .AndFactIs("value", items[2]);

                list.Add(new IdentifiableParam
                {
                    Id = items[0],
                    Key = items[1],
                    Value = valValue
                });
            }

            return list.ToArray();
        }

        public class IdentifiableParam
        {
            public string Id { get; set; }

            public string Key { get; set; }
            public long Value { get; set; }
        }
    }
}