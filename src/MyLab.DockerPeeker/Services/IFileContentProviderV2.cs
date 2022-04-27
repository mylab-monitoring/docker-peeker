using System.IO;
using System.Threading.Tasks;

namespace MyLab.DockerPeeker.Services
{
    public interface IFileContentProviderV2
    {
        Task<string> ReadCpuStat(string containerLongId);
        Task<string> ReadIoStat(string containerLongId);

        Task<string> ReadNetStat(string containerPid);

        Task<string> ReadMemInfo();
    }

    class FileContentProviderV2 : IFileContentProviderV2
    {
        public Task<string> ReadCpuStat(string containerLongId)
        {
            return File.ReadAllTextAsync($"/etc/docker-peeker/cgroup/system.slice/docker-{containerLongId}.scope/cpu.stat");
        }

        public Task<string> ReadIoStat(string containerLongId)
        {
            return File.ReadAllTextAsync($"/etc/docker-peeker/cgroup/system.slice/docker-{containerLongId}.scope/io.stat");
        }

        public Task<string> ReadNetStat(string containerPid)
        {
            return File.ReadAllTextAsync($"/etc/docker-peeker/proc/{containerPid}/net/dev"); 
        }

        public Task<string> ReadMemInfo()
        {
            return File.ReadAllTextAsync("/proc/meminfo");
        }
    }
}
