using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Tools;
using MyLab.Logging;

namespace MyLab.DockerPeeker.Services
{
    public interface IContainerListProvider
    {
        Task<ContainerLink[]> ProviderActiveContainersAsync();
    }

    class ContainerListProvider : IContainerListProvider
    {
        public async Task<ContainerLink[]> ProviderActiveContainersAsync()
        {
            var lines = await DockerCaller.GetActiveContainersAsync();
            return lines.Select(ContainerLink.Read).ToArray();
        }
    }

    public class ContainerLink
    {
        public string LongId { get; set; }
        public string Name { get; set; }

        public static ContainerLink Read(string containerLink)
        {
            if (containerLink == null) 
                throw new ArgumentNullException(nameof(containerLink));
            if (string.IsNullOrWhiteSpace(containerLink))
                throw new FormatException("Container link is empty or whitespace");

            var separatorIndex = containerLink.IndexOf('\t');
            if(separatorIndex < 0)
                throw new FormatException("Container link separator not found")
                    .AndFactIs("Link", containerLink);

            return new ContainerLink
            {
                LongId = containerLink.Remove(separatorIndex),
                Name = containerLink.Substring(separatorIndex+1)
            };
        }
    }
}
