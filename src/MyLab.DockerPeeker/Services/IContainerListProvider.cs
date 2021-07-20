using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyLab.DockerPeeker.Tools;
using MyLab.Log;
using MyLab.Log.Dsl;

namespace MyLab.DockerPeeker.Services
{
    public interface IContainerListProvider
    {
        Task<ContainerLink[]> ProviderActiveContainersAsync();
    }

    class ContainerListProvider : IContainerListProvider
    {
        private readonly DockerCaller _dockerCaller;

        public ContainerListProvider(ILogger<ContainerListProvider> logger)
        {
            _dockerCaller = new DockerCaller
            {
                Logger = logger.Dsl()
            };
        }

        public async Task<ContainerLink[]> ProviderActiveContainersAsync()
        {
            var lines = await _dockerCaller.GetActiveContainersAsync();
            return lines.Select(ContainerLink.Read).ToArray();
        }
    }

    public class ContainerLink
    {
        public string LongId { get; set; }
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public static ContainerLink Read(string containerLink)
        {
            if (containerLink == null) 
                throw new ArgumentNullException(nameof(containerLink));
            if (string.IsNullOrWhiteSpace(containerLink))
                throw new FormatException("Container link is empty or whitespace");

            var separator1Index = containerLink.IndexOf('\t');
            if(separator1Index < 0)
                throw new FormatException("Container link separator-1 not found")
                    .AndFactIs("Link", containerLink);

            var separator2Index = containerLink.LastIndexOf('\t');
            if (separator2Index < 0)
                throw new FormatException("Container link separator-2 not found")
                    .AndFactIs("Link", containerLink);

            var dtItems = containerLink.Substring(separator2Index + 1).Split(' ');
            if(dtItems.Length != 4)
                throw new FormatException("CreatedAt value has wrong format")
                    .AndFactIs("Link", containerLink)
                    .AndFactIs("CratedAt", dtItems);
            
            var createdAt = DateTime.Parse($"{dtItems[0]} {dtItems[1]} {dtItems[2]}");

            return new ContainerLink
            {
                LongId = containerLink.Remove(separator1Index).Trim(),
                Name = containerLink.Substring(separator1Index+1, separator2Index-(separator1Index+1)).Trim(),
                CreatedAt = createdAt
            };
        }
    }
}
