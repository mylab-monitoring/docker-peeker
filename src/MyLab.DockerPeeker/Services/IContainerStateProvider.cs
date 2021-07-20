using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyLab.DockerPeeker.Tools;
using MyLab.Log.Dsl;

namespace MyLab.DockerPeeker.Services
{
    public interface IContainerStateProvider
    {
        Task<ContainerState[]> ProvideAsync(ContainerLink[] containersLinks);
    }

    class DockerContainerStateProvider : IContainerStateProvider
    {
        private readonly IDictionary<string, CashedState> _states = new Dictionary<string, CashedState>();
        private readonly object _statesLock = new object();
        private readonly DockerCaller _dockerCaller;

        public DockerContainerStateProvider(ILogger<DockerContainerStateProvider> logger)
        {
            _dockerCaller = new DockerCaller
            {
                Logger = logger.Dsl()
            };
        }

        public async Task<ContainerState[]> ProvideAsync(ContainerLink[] containersLinks)
        {
            string[] needToGetIds;

            lock (_statesLock)
            {
                needToGetIds = containersLinks
                    .Where(l => !_states.TryGetValue(l.LongId, out var found) || found.ActualDt < l.CreatedAt)
                    .Select(l => l.LongId)
                    .ToArray();
            }

            if (needToGetIds.Length != 0)
            {
                var dockerResponse = await _dockerCaller.GetStates(needToGetIds);

                var newStates = dockerResponse
                    .Select(ContainerState.Parse)
                    .ToArray();

                lock (_statesLock)
                {
                    foreach (var newState in newStates)
                    {
                        var newCashedState = new CashedState
                        {
                            ActualDt = DateTime.Now,
                            State = newState
                        };

                        if (_states.ContainsKey(newState.Id))
                        {
                            _states[newState.Id] = newCashedState;
                        }
                        else
                        {
                            _states.Add(newState.Id, newCashedState);
                        }
                    }
                }
            }

            lock (_statesLock)
            {
                return containersLinks
                    .Where(l => _states.ContainsKey(l.LongId))
                    .Select(l => _states[l.LongId].State)
                    .ToArray();
            }
        }

        class CashedState
        {
            public ContainerState State { get; set; }
            public DateTime ActualDt { get; set; }
        }
    }
}