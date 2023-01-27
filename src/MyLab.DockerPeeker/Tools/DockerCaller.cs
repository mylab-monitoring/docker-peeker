using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Options;

namespace MyLab.DockerPeeker.Tools
{
    class DockerCaller
    {
        private readonly DockerClient _docker;
        private readonly ServiceLabelExcludeLogic _labelExcludeLogic;

        /// <summary>
        /// Initializes a new instance of <see cref="DockerCaller"/>
        /// </summary>
        public DockerCaller(IOptions<DockerPeekerOptions> opts)
        {
            _docker = new DockerClientConfiguration(
                    new Uri(opts.Value.Socket))
                .CreateClient();

            _labelExcludeLogic = opts.Value.DisableServiceContainerLabels
                ? new ServiceLabelExcludeLogic(opts.Value.ServiceLabelsWhiteList)
                : null;
        }

        public async Task<ContainerShortInfo[]> GetContainersAsync()
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

                var selectedLabels = _labelExcludeLogic != null
                    ? inspection.Config.Labels.Where(l => !_labelExcludeLogic.ShouldExcludeLabel(l.Key))
                    : inspection.Config.Labels;

                resList.Add(new ContainerState
                {
                    Name = inspection.Name.TrimStart('/'),
                    Id = containerId,
                    Pid = inspection.State.Pid.ToString(),
                    Labels = new Dictionary<string, string>(selectedLabels),
                    Status = inspection.State.Status,
                    IsActive = inspection.State.Running
                });
            }

            return resList.ToArray();
        }
    }
}
