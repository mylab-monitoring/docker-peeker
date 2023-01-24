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
        Task<ContainerState[]> ProvideAsync();
    }

    class DockerContainerStateProvider : IContainerStateProvider
    {
        private readonly IDictionary<string, CashedState> _states = new Dictionary<string, CashedState>();
        private readonly object _statesLock = new ();
        private readonly DockerCaller _dockerCaller;
        private readonly IDslLogger _log;

        public DockerContainerStateProvider(DockerCaller dockerCaller, ILogger<DockerContainerStateProvider> logger = null)
        {
            _log = logger?.Dsl();
            _dockerCaller = dockerCaller;
        }

        public async Task<ContainerState[]> ProvideAsync()
        {
            string[] needToGetState;

            var containers = await _dockerCaller.GetActiveContainersAsync();

            var lostContainers = _states.Keys.Where(ck => containers.All(c => c.LongId != ck));
            foreach (var lostContainer in lostContainers)
            {
                _states.Remove(lostContainer);
            } 

            lock (_statesLock)
            {
                needToGetState = containers
                    .Where(l => !_states.TryGetValue(l.LongId, out var found) || found.ActualDt < l.CreatedAt)
                    .Select(l => l.LongId)
                    .ToArray();
            }

            if (needToGetState.Length != 0)
            {
                var dockerStates = await _dockerCaller.GetStates(needToGetState);
                
                lock (_statesLock)
                {
                    foreach (var newState in dockerStates)
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
                return containers
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