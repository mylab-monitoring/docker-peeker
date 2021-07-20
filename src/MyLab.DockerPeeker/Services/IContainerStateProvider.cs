using System.Linq;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Tools;

namespace MyLab.DockerPeeker.Services
{
    public interface IContainerStateProvider
    {
        Task<ContainerState[]> ProvideAsync(string[] containersIds);
    }

    class DockerContainerStateProvider : IContainerStateProvider
    {
        public async Task<ContainerState[]> ProvideAsync(string[] containersIds)
        {
            var dockerResponse = await DockerCaller.GetLabels(containersIds);
            return dockerResponse.Select(ContainerState.Parse).ToArray();
        }
    }
}