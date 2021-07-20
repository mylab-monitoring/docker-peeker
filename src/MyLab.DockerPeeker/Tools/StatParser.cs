using System;
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

        public NetParam[] ExtractNetParams()
        {
            if(_lines.Length<=2)
                return new NetParam[0];

            return _lines.Skip(2).Where(s => !string.IsNullOrWhiteSpace(s)).Select(ParseNetStatLine).ToArray();
        }

        private NetParam ParseNetStatLine(string netStatLine)
        {
            var items = netStatLine
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            if(items.Length != 17)
                throw new PseudoFileFormatException("Net stat line items wrong count")
                    .AndFactIs("Stat line", netStatLine)
                    .AndFactIs("Detected count", items.Length);

            try
            {
                return new NetParam
                {
                    Interface = items[0].TrimEnd(':'),
                    ReceiveBytes = long.Parse(items[1]),
                    ReceivePackets = long.Parse(items[2]),
                    ReceiveErrors = long.Parse(items[3]),
                    ReceiveDrop = long.Parse(items[4]),
                    ReceiveFifo = long.Parse(items[5]),
                    ReceiveFrame = long.Parse(items[6]),
                    ReceiveCompressed = long.Parse(items[7]),
                    ReceiveMulticast = long.Parse(items[8]),
                    TransmitBytes = long.Parse(items[9]),
                    TransmitPackets = long.Parse(items[10]),
                    TransmitErrors = long.Parse(items[11]),
                    TransmitDrop = long.Parse(items[12]),
                    TransmitFifo = long.Parse(items[13]),
                    TransmitColls = long.Parse(items[14]),
                    TransmitCarrier = long.Parse(items[15]),
                    TransmitCompressed = long.Parse(items[16])
                };
            }
            catch (Exception e)
            {
                throw new PseudoFileFormatException("Net stat line parsing error", e)
                    .AndFactIs("Stat line", netStatLine);
            }
        }

        public class NetParam
        {
            public string Interface { get; set; }

            public long ReceiveBytes { get; set; }
            public long ReceivePackets { get; set; }
            public long ReceiveErrors { get; set; }
            public long ReceiveDrop { get; set; }
            public long ReceiveFifo { get; set; }
            public long ReceiveFrame { get; set; }
            public long ReceiveCompressed { get; set; }
            public long ReceiveMulticast { get; set; }

            public long TransmitBytes { get; set; }
            public long TransmitPackets { get; set; }
            public long TransmitErrors { get; set; }
            public long TransmitDrop { get; set; }
            public long TransmitFifo { get; set; }
            public long TransmitColls { get; set; }
            public long TransmitCarrier { get; set; }
            public long TransmitCompressed { get; set; }

        }

        public class IdentifiableParam
        {
            public string Id { get; set; }

            public string Key { get; set; }
            public long Value { get; set; }
        }
    }
}