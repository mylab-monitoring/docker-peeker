using System.Threading.Tasks;

namespace MyLab.DockerPeeker.Tools
{
    interface IPseudoFileProvider
    {
        Task<string> LoadCpuAcct();

    }
}