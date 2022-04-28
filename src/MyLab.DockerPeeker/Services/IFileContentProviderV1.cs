using System.IO;
using System.Threading.Tasks;

namespace MyLab.DockerPeeker.Services
{
    public interface IFileContentProviderV1 : ICommonFileContentProvider
    {
        Task<string> ReadCpuStat(string containerLongId);
        Task<string> ReadMemStat(string containerLongId);
        Task<string> ReadBlkStat(string containerLongId);
    }

    class FileContentProviderV1 : IFileContentProviderV1
    {
        public Task<string> ReadCpuStat(string containerLongId)
        {
            return File.ReadAllTextAsync($"/etc/docker-peeker/cgroup/cpuacct/docker/{containerLongId}/cpuacct.stat");
        }

        public Task<string> ReadMemStat(string containerLongId)
        {
            return File.ReadAllTextAsync($"/etc/docker-peeker/cgroup/memory/docker/{containerLongId}/memory.stat");
        }

        public Task<string> ReadBlkStat(string containerLongId)
        {
            return File.ReadAllTextAsync($"/etc/docker-peeker/cgroup/blkio/docker/{containerLongId}/blkio.throttle.io_service_bytes");
        }

        public Task<string> ReadNetStat(string containerPid)
        {
            return File.ReadAllTextAsync($"/etc/docker-peeker/proc/{containerPid}/net/dev"); 
        }
    }
}
