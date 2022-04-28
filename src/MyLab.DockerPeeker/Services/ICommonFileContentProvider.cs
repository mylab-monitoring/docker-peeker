using System.Threading.Tasks;

namespace MyLab.DockerPeeker.Services
{
    public interface ICommonFileContentProvider
    {
        Task<string> ReadNetStat(string containerPid);
    }
}