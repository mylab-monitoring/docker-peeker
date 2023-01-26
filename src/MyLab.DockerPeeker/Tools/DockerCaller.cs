using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public async Task<ContainerShortInfo[]> GetActiveContainersAsync()
        {
            var containers = await _docker.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true
            });

            return containers.Select(c => new ContainerShortInfo(c)).ToArray();
        }

        public async Task<ContainerState[]> GetStates(string[] ids)
        {
            var resList = new List<ContainerState>();

            foreach (var containerId in ids)
            {
                var inspection = await _docker.Containers.InspectContainerAsync(containerId);
                
                resList.Add(new ContainerState
                {
                    Name = inspection.Name,
                    ContainerId = containerId,
                    Pid = inspection.State.Pid.ToString(),
                    Labels = new Dictionary<string, string>(inspection.Config.Labels)
                });
            }

            return resList.ToArray();
        }
    }
}
