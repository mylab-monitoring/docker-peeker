using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Tools;

namespace MyLab.DockerPeeker.Services
{
    class ContainersLabelsSource
    {
        private readonly IContainerLabelsProvider _containerLabelsProvider;
        readonly IDictionary<string, ContainerLabels> _labelsMap = new Dictionary<string, ContainerLabels>();

        public ContainersLabelsSource(IContainerLabelsProvider containerLabelsProvider)
        {
            _containerLabelsProvider = containerLabelsProvider;
        }

        public async Task<ContainerLabels[]> GetContainerLabels(string[] containersIds)
        {
            var newContainers = containersIds
                .Where(id => !_labelsMap.Keys.Contains(id))
                .ToArray();

            if (newContainers.Length != 0)
            {
                var containerLabelsList = await _containerLabelsProvider.ProvideAsync(newContainers);
                foreach (var containerLabels in containerLabelsList)
                {
                    _labelsMap.TryAdd(containerLabels.ContainerId, containerLabels);
                }
            }

            var res = new List<ContainerLabels>();

            foreach (var containersId in containersIds)
            {
                if (_labelsMap.TryGetValue(containersId, out var foundLabels))
                {
                    res.Add(foundLabels);
                }
            }

            return res.ToArray();
        }

    }
}

