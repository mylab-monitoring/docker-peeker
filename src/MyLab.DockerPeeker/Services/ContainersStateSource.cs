using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Tools;

namespace MyLab.DockerPeeker.Services
{
    class ContainersStateSource
    {
        private readonly IContainerStateProvider _containerStateProvider;
        readonly IDictionary<string, ContainerState> _stateMap = new Dictionary<string, ContainerState>();

        public ContainersStateSource(IContainerStateProvider containerStateProvider)
        {
            _containerStateProvider = containerStateProvider;
        }

        public async Task<ContainerState[]> GetContainerStates(string[] containersIds)
        {
            var newContainers = containersIds
                .Where(id => !_stateMap.Keys.Contains(id))
                .ToArray();

            if (newContainers.Length != 0)
            {
                var containerStateList = await _containerStateProvider.ProvideAsync(newContainers);
                foreach (var containerState in containerStateList)
                {
                    _stateMap.TryAdd(containerState.Id, containerState);
                }
            }

            var res = new List<ContainerState>();

            foreach (var containersId in containersIds)
            {
                if (_stateMap.TryGetValue(containersId, out var foundLabels))
                {
                    res.Add(foundLabels);
                }
            }

            return res.ToArray();
        }

    }
}

