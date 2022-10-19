using System;
using System.IO;
using System.Threading.Tasks;

namespace MyLab.DockerPeeker.Services
{
    interface ICGroupDetector
    {
        Task<CGroupVersion> GetCGroupVersionAsync();
    }

    enum CGroupVersion
    {
        Undefined,
        V1,
        V2
    }

    class CGroupDetector : ICGroupDetector
    {
        public async Task<CGroupVersion> GetCGroupVersionAsync()
        {
            const string fSysName = "/proc/filesystems";

            if (!File.Exists(fSysName))
                throw new InvalidOperationException("CGroup ver detection error: file '/proc/filesystems' not found");

            var fileContent = await File.ReadAllTextAsync(fSysName);

            return fileContent.Contains("cgroup2")
                ? CGroupVersion.V2
                : CGroupVersion.V1;
        }
    }
}
