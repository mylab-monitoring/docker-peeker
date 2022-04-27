using System;
using System.Collections.Generic;
using System.Linq;
using MyLab.Log;

namespace MyLab.DockerPeeker.Tools.StatObjectModel
{
    public class NetStat : Dictionary<string, NetStat.NetStatItem>
    {
        public NetStat()
        {
            
        }

        public NetStat(IDictionary<string, NetStat.NetStatItem> initial)
            :base(initial)
        {

        }

        public NetStat(IEnumerable<KeyValuePair<string, NetStat.NetStatItem>> initial)
            :base(initial)
        {

        }

        public static NetStat Parse(string content)
        {
            var lines = StatObjectModelTools.SplitLines(content).ToArray();

            if (lines.Length < 2)
                return new NetStat();

            var kvPairs = lines
                .Skip(2)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(ParseNetStatLine);

            return new NetStat(kvPairs);

        }

        private static KeyValuePair<string, NetStatItem> ParseNetStatLine(string netStatLine)
        {
            var items = netStatLine.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            if (items.Length != 17)
                throw new FormatException("Net stat line items wrong number")
                    .AndFactIs("line", netStatLine)
                    .AndFactIs("actual-number", items.Length);

            try
            {
                return new KeyValuePair<string, NetStatItem>(
                    items[0].TrimEnd(':'),
                    new NetStatItem
                {
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
                });
            }
            catch (Exception e)
            {
                throw new FormatException("Net stat line parsing error", e)
                    .AndFactIs("line", netStatLine);
            }
        }

        public class NetStatItem
        {
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
    }
}
