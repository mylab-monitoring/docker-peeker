using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyLab.DockerPeeker.Services
{
    public interface IFileContentProvider
    {
        Task<string> ReadCpuAcctStat(string containerLongId);
        Task<string> ReadMemStat(string containerLongId);
        Task<string> ReadBlkIoServiceBytesStat(string containerLongId);

        Task<string> ReadNetStat(string containerPid);
    }

    class RealFileContentProvider : IFileContentProvider
    {
        public Task<string> ReadCpuAcctStat(string containerLongId)
        {
            return File.ReadAllTextAsync($"/etc/docker-peeker/cgroup/cpuacct/docker/{containerLongId}/cpuacct.stat");
        }

        public Task<string> ReadMemStat(string containerLongId)
        {
            return File.ReadAllTextAsync($"/etc/docker-peeker/cgroup/memory/docker/{containerLongId}/memory.stat");
        }

        public Task<string> ReadBlkIoServiceBytesStat(string containerLongId)
        {
            return File.ReadAllTextAsync($"/etc/docker-peeker/cgroup/blkio/docker/{containerLongId}/blkio.throttle.io_service_bytes");
        }

        public Task<string> ReadNetStat(string containerPid)
        {
            return File.ReadAllTextAsync($"/etc/docker-peeker/proc/{containerPid}/net/dev"); 
        }
    }
}
