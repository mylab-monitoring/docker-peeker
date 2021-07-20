using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyLab.DockerPeeker.Services
{
    public interface IFileContentProvider
    {
        Task<string> ReadCpuAcct(string containerLongId);
    }

    class RealFileContentProvider : IFileContentProvider
    {
        public Task<string> ReadCpuAcct(string containerLongId)
        {
            return File.ReadAllTextAsync($"/sys/fs/cgroup/cpuacct/docker/{containerLongId}/cpuacct.stat");
        }
    }
}
