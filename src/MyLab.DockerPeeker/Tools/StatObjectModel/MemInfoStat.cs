using System;
using System.Linq;
using MyLab.Log;

namespace MyLab.DockerPeeker.Tools.StatObjectModel
{
    public class MemInfoStat
    {
        public long MemTotal { get; set; }
        public long SwapTotal { get; set; }

        public static MemInfoStat ParseMemInfo(string content)
        {
            var lines = StatObjectModelTools.SplitLines(content).ToArray();

            var memTotalLine = lines.FirstOrDefault(l => l.StartsWith("MemTotal:"));
            if (memTotalLine == null)
                throw new FormatException("MemTotal line not found");

            var swapTotalLine = lines.FirstOrDefault(l => l.StartsWith("SwapTotal:"));
            if (memTotalLine == null)
                throw new FormatException("SwapTotal line not found");

            return new MemInfoStat
            {
                MemTotal = ExtractValue(memTotalLine)*1024,
                SwapTotal = ExtractValue(swapTotalLine)*1024
            };
        }

        static long ExtractValue(string line)
        {
            var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            try
            {
                if (parts.Length != 3)
                {
                    if (parts.Length == 2 && parts[1] == "0")
                        return 0;

                    throw new FormatException("Wrong number of line parts. It should be 3 if value is zero ('0')")
                        .AndFactIs("actual-part-number", parts.Length);

                }

                if (parts[2] != "kB")
                    throw new NotSupportedException("Dimension not supported")
                        .AndFactIs("dimension", parts[2]);

                return long.Parse(parts[1]);
            }
            catch (Exception e)
            {
                e.AndFactIs("line", line);
                throw;
            }
        }
    }
}
