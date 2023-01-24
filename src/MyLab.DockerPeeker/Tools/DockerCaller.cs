using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace MyLab.DockerPeeker.Tools
{
    class DockerCaller
    {
        private readonly DockerClient _docker;

        /// <summary>
        /// Initializes a new instance of <see cref="DockerCaller"/>
        /// </summary>
        public DockerCaller()
        {
            _docker = new DockerClientConfiguration(
                    new Uri("unix:///var/run/docker.sock"))
                .CreateClient();
        }

        public async Task<ContainerLink[]> GetActiveContainersAsync()
        {
            var containers = await _docker.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true
            });

            return containers.Select(c => new ContainerLink
            {
                CreatedAt = c.Created,
                LongId = c.ID,
                Name = c.Names.FirstOrDefault()
            }).ToArray();
        }

        public async Task<ContainerState[]> GetStates(string[] ids)
        {
            var containers = await _docker.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true
            });

            var resList = new List<ContainerState>();

            var selectedContainers = containers.Where(c => ids.Contains(c.ID));

            foreach (var container in selectedContainers)
            {
                var inspection = await _docker.Containers.InspectContainerAsync(container.ID);
                
                resList.Add(new ContainerState(container.ID, inspection.State.Pid.ToString(), container.Labels));
            }

            return resList.ToArray();
        }
    }
}
