using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyLab.DockerPeeker.Tools
{
    public interface IContainerMetricsProvider
    {
        Task<IEnumerable<ContainerMetric>> ProvideAsync(string containerLongId);
    }
}