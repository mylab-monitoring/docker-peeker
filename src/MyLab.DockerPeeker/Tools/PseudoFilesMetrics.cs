using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MyLab.Logging;

namespace MyLab.DockerPeeker.Tools
{
    class PseudoFilesMetrics
    {
        public CpuAcctStat CpuStat { get; set; }

        public static async Task<PseudoFilesMetrics> Load(IPseudoFileProvider pseudoFileProvider)
        {
            var cpuacctContent = await pseudoFileProvider.LoadCpuAcct();

            return new PseudoFilesMetrics
            {
                CpuStat = CpuAcctStat.Parse(cpuacctContent)
            };
        }
    }

    class CpuAcctStat
    {
        public long User { get; set; }

        public long System { get; set; }

        public static CpuAcctStat Parse(string stringContent)
        {
            var lines = stringContent.Split('\n');

            var user = lines.FirstOrDefault(l => l.StartsWith("user "));
            if (user == null)
                throw new PseudoFileFormatException("User cpu not found");
            var userStringVal = user.Substring(5).Trim();
            if (!long.TryParse(userStringVal, NumberStyles.Any, CultureInfo.InvariantCulture, out var userValue))
                throw new PseudoFileFormatException("User cpu has wrong format")
                    .AndFactIs("value", userStringVal);


            var system  = lines.FirstOrDefault(l => l.StartsWith("system "));
            if (system == null)
                throw new PseudoFileFormatException("System cpu not found");
            var systemStringVal = system.Substring(7).Trim();
            if (!long.TryParse(systemStringVal, NumberStyles.Any, CultureInfo.InvariantCulture, out var systemValue))
                throw new PseudoFileFormatException("System cpu has wrong format")
                    .AndFactIs("value", systemStringVal);

            return new CpuAcctStat
            {
                User = userValue,
                System = systemValue
            };

        }
    }
}
