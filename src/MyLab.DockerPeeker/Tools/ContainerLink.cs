using System;
using System.Linq;
using Docker.DotNet.Models;

namespace MyLab.DockerPeeker.Tools
{
    public class ContainerShortInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ContainerShortInfo"/>
        /// </summary>
        public ContainerShortInfo()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ContainerShortInfo"/>
        /// </summary>
        public ContainerShortInfo(ContainerListResponse response)
        {
            Id = response.ID;
            CreatedAt = response.Created;
            Name = response.Names.FirstOrDefault();
        }
    }
}