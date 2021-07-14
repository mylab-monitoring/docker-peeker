using System.Collections.Generic;
using System.Threading.Tasks;

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


}
