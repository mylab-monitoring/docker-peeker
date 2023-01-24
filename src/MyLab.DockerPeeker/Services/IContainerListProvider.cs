using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyLab.DockerPeeker.Tools;
using MyLab.Log;
using MyLab.Log.Dsl;

namespace MyLab.DockerPeeker.Services
{
    //public interface IContainerListProvider
    //{
    //    Task<ContainerLink[]> ProviderActiveContainersAsync();
    //}

    //class ContainerListProvider : IContainerListProvider
    //{
    //    private readonly DockerCaller _dockerCaller;

    //    public ContainerListProvider(DockerCaller dockerCaller, ILogger<ContainerListProvider> logger)
    //    {
    //        _dockerCaller = dockerCaller;
    //    }

    //    public async Task<ContainerLink[]> ProviderActiveContainersAsync()
    //    {
    //        return await _dockerCaller.GetActiveContainersAsync();
    //    }
    //}
}
