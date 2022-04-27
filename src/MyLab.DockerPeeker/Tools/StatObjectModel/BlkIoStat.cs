using System;
using System.Collections.Generic;
using System.Linq;
using MyLab.Log;

namespace MyLab.DockerPeeker.Tools.StatObjectModel
{
    public class BlkIoStat : Dictionary<string, BlkIoStat.BlkIoStatItem>
    {
        public BlkIoStat()
        {
            
        }

        public BlkIoStat(IDictionary<string, BlkIoStat.BlkIoStatItem> initial)
            : base(initial)
        {
            
        }

        public BlkIoStat(IEnumerable<KeyValuePair<string, BlkIoStat.BlkIoStatItem>> initial)
            : base(initial)
        {

        }

        public static BlkIoStat ParseV1(string content)
        {
            var dict = StatObjectModelTools.SplitLines(content)
                .Where(l => !l.StartsWith("Total "))
                .Select(l =>
                {
                    var parts = l.Split(" ");

                    try
                    {
                        if (parts.Length != 3)
                            throw new FormatException("The line must be three parts");

                        return (parts[0], parts[1], long.Parse(parts[2]));
                    }
                    catch (Exception e)
                    {
                        e.AndFactIs("line", l);
                        throw;
                    }
                })
                .GroupBy(l => l.Item1)
                .ToDictionary(g => g.Key, g =>
                {
                    var read = g.FirstOrDefault(l => l.Item2 == "Read").Item3;
                    var write = g.FirstOrDefault(l => l.Item2 == "Write").Item3;

                    return new BlkIoStatItem
                    {
                        Read = read,
                        Write = write
                    };
                });

            return new BlkIoStat(dict);
        }

        public static BlkIoStat ParseV2(string content)
        {
            return new BlkIoStat(StatObjectModelTools.SplitLines(content).Select(ParseLine));

            KeyValuePair<string, BlkIoStatItem> ParseLine(string line)
            {
                var lArr = line.Split(' ');

                if (lArr.Length != 7)
                    throw new FormatException("IO Stat line wrong number of parts")
                        .AndFactIs("part-number", lArr.Length)
                        .AndFactIs("line", line);

                var dictParams = lArr.Skip(1).Select(l =>
                {

                    try
                    {
                        return ParsePair(l);
                    }
                    catch (Exception e)
                    {
                        throw new FormatException("IO Stat pair wrong format", e)
                            .AndFactIs("line", line)
                            .AndFactIs("pair", l);
                    }

                }).ToDictionary(l => l.Key, l => l.Value);

                return new KeyValuePair<string, BlkIoStatItem>(
                    lArr[0], 
                    new BlkIoStatItem
                    {
                        Read = dictParams["rbytes"],
                        Write = dictParams["wbytes"]
                    }
                );
            }

            (string Key, long Value) ParsePair(string pair)
            {
                var delimiterIndex = pair.IndexOf('=');
                if (delimiterIndex == -1)
                    throw new InvalidOperationException("Delimiter not found");

                var resultKey = pair.Remove(delimiterIndex);
                var resultValue = long.Parse(pair.Substring(delimiterIndex + 1));

                return (resultKey, resultValue);
            }
        }

        public class BlkIoStatItem
        {
            public long Read { get; set; }
            public long Write { get; set; }
        }
    }
}
