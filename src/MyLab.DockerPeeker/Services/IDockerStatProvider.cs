using System;
using System.Linq;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Tools;

namespace MyLab.DockerPeeker.Services
{
    public interface IDockerStatProvider
    {
        Task<DockerStatItem[]> Provide();
    }

    class DockerStatProvider : IDockerStatProvider
    {
        public async Task<DockerStatItem[]> Provide()
        {
            var ids = await DockerCaller.GetActiveContainersIds();



            //return dockerResponse.Select(DockerStatItem.Parse).ToArray();

            throw new NotImplementedException();
        }
    }
}