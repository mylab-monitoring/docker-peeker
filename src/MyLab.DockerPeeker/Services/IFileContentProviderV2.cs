using System.IO;
using System.Threading.Tasks;

namespace MyLab.DockerPeeker.Services
{
    public interface IFileContentProviderV2 : ICommonFileContentProvider
    {
        Task<string> ReadCpuStat(string containerLongId);
        Task<string> ReadIoStat(string containerLongId);
        Task<string> ReadMemInfo();
        Task<string> ReadMemStat(string containerLongId);
        Task<string> ReadMemSwapCurrent(string containerLongId);

        Task<string> ReadMemMax(string containerLongId);
        Task<string> ReadSwapMax(string containerLongId);
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

        public Task<string> ReadMemStat(string containerLongId)
        {
            return File.ReadAllTextAsync($"/etc/docker-peeker/cgroup/system.slice/docker-{containerLongId}.scope/memory.stat");
        }

        public Task<string> ReadMemSwapCurrent(string containerLongId)
        {
            return File.ReadAllTextAsync($"/etc/docker-peeker/cgroup/system.slice/docker-{containerLongId}.scope/memory.swap.current");
        }

        public Task<string> ReadMemMax(string containerLongId)
        {
            return File.ReadAllTextAsync($"/etc/docker-peeker/cgroup/system.slice/docker-{containerLongId}.scope/memory.max");
        }

        public Task<string> ReadSwapMax(string containerLongId)
        {
            return File.ReadAllTextAsync($"/etc/docker-peeker/cgroup/system.slice/docker-{containerLongId}.scope/memory.swap.max");
        }
    }
}
