﻿using System;
using System.Collections.Generic;
using System.IO;
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

            var containers = await _dockerCaller.GetContainersAsync();
            
            lock (_statesLock)
            {
                var lostContainers = _states.Keys.Where(ck => containers.All(c => c.Id != ck));

                ActualizeCache(containers.Select(c => c.Id).ToArray());

                foreach (var lostContainer in lostContainers)
                {
                    _states.Remove(lostContainer);
                }
            }

            lock (_statesLock)
            {
                needToGetState = containers
                    .Where(l => 
                        !_states.TryGetValue(l.Id, out var found) || 
                        found.ActualDt < l.CreatedAt)
                    .Select(l => l.Id)
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
                            State = newState,
                            Pid = newState.Pid
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
                    .Where(l => _states.ContainsKey(l.Id))
                    .Select(l => _states[l.Id].State)
                    .ToArray();
            }
        }

        private void ActualizeCache(string[] actualContainerIds)
        {
            var cache = _states.ToArray();

            foreach (var itm in cache)
            {
                if (!actualContainerIds.Contains(itm.Key) ||
                    !Directory.Exists($"/proc/{itm.Value.Pid}"))
                {
                    _states.Remove(itm.Key);
                }
            }
        }

        class CashedState
        {
            public ContainerState State { get; set; }

            public DateTime ActualDt { get; set; }

            public string Pid { get; set; }
        }
    }
}