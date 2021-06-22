using System.Linq;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Tools;

namespace MyLab.DockerPeeker.Services
{
    public interface IContainerLabelsProvider
    {
        Task<ContainerLabels[]> Provide(string[] containersIds);
    }

    class DockerContainerLabelsProvider : IContainerLabelsProvider
    {
        public async Task<ContainerLabels[]> Provide(string[] containersIds)
        {
            var dockerResponse = await DockerCaller.GetLabels(containersIds);
            return dockerResponse.Select(ContainerLabels.Parse).ToArray();
        }
    }
}