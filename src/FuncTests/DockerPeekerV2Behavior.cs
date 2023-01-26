using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.ApiClient.Test;
using MyLab.DockerPeeker;
using MyLab.DockerPeeker.Services;
using MyLab.DockerPeeker.Tools;
using Xunit;
using Xunit.Abstractions;

namespace FuncTests
{
    public class DockerPeekerV2Behavior : IDisposable
    {
        private readonly TestApi<Startup, IMetricService> _api;

        public DockerPeekerV2Behavior(ITestOutputHelper output)
        {
            _api = new TestApi<Startup, IMetricService>
            {
                Output = output,
                ServiceOverrider = s => s
                    .AddSingleton<IFileContentProviderV2, TestFileContentProvider>()
                    .AddSingleton<ICGroupDetector, TestCGroupDetector>()
                    .AddSingleton<IContainerStateProvider, TestContainerStateProvider>()
                    .AddLogging(l => l
                        .AddFilter(l => true)
                        .AddXUnit(output)
                    )
            };
        }

        [Fact]
        public async Task ShouldProvideMetrics()
        {
            //Arrange
            var client = _api.Start();

            //Act
            var res = await client.Call(s => s.GetMetrics());

            //Assert
            Assert.NotNull(res.ResponseContent);
        }

        [Fact]
        public async Task ShouldProvideCpuAcctStat()
        {
            //Arrange
            var client = _api.StartWithProxy();

            //Act
            var metrics = (await client.GetMetrics())
                .Split('\n')
                .Select(s => s.Trim())
                .ToArray();

            //Assert
            Assert.Contains("container_cpu_jiffies_total{container_name=\"bar\",mode=\"user\",container_label_label_pid=\"123\"} 8497", metrics);
            Assert.Contains("container_cpu_jiffies_total{container_name=\"bar\",mode=\"system\",container_label_label_pid=\"123\"} 9226",
                metrics);
        }

        [Fact]
        public async Task ShouldProvideMemStat()
        {
            //Arrange
            var client = _api.StartWithProxy();

            //Act
            var metrics = (await client.GetMetrics())
                .Split('\n')
                .Select(s => s.Trim())
                .ToArray();

            //Assert
            Assert.Contains("container_mem_bytes{container_name=\"bar\",type=\"swap\",container_label_label_pid=\"123\"} 16113664", metrics);
            Assert.Contains("container_mem_bytes{container_name=\"bar\",type=\"cache\",container_label_label_pid=\"123\"} 38793216", metrics);
            Assert.Contains("container_mem_bytes{container_name=\"bar\",type=\"rss\",container_label_label_pid=\"123\"} 120168448", metrics);
            Assert.Contains("container_mem_limit_bytes{container_name=\"bar\",type=\"ram\",container_label_label_pid=\"123\"} 9223372036854775807", metrics);
            Assert.Contains("container_mem_limit_bytes{container_name=\"bar\",type=\"ramswap\",container_label_label_pid=\"123\"} 9223372036854775807", metrics);
        }

        [Fact]
        public async Task ShouldProvideBlockIdStat()
        {
            //Arrange
            var client = _api.StartWithProxy();

            //Act
            var metrics = (await client.GetMetrics())
                .Split('\n')
                .Select(s => s.Trim())
                .ToArray();

            //Assert
            Assert.Contains("container_blk_bytes_total{container_name=\"bar\",direction=\"read\",container_label_label_pid=\"123\"} 1007616", metrics);
            Assert.Contains("container_blk_bytes_total{container_name=\"bar\",direction=\"write\",container_label_label_pid=\"123\"} 8192", metrics);
        }

        [Fact]
        public async Task ShouldProvideNetStat()
        {
            //Arrange
            var client = _api.StartWithProxy();

            //Act
            var metrics = (await client.GetMetrics())
                .Split('\n')
                .Select(s => s.Trim())
                .ToArray();

            //Assert
            Assert.Contains("container_net_bytes_total{container_name=\"bar\",direction=\"receive\",container_label_label_pid=\"123\"} 33373764", metrics);
            Assert.Contains("container_net_bytes_total{container_name=\"bar\",direction=\"transmit\",container_label_label_pid=\"123\"} 33373774", metrics);
        }

        [Fact]
        public async Task ShouldExpireContainerState()
        {
            //Arrange
            var client = _api.StartWithProxy();

            //Act
            var metrics1 = (await client.GetMetrics())
                .Split('\n')
                .Select(s => s.Trim())
                .ToArray();

            var metrics2 = (await client.GetMetrics())
                .Split('\n')
                .Select(s => s.Trim())
                .ToArray();

            //Assert
            Assert.Contains("container_blk_bytes_total{container_name=\"bar\",direction=\"write\",container_label_label_pid=\"123\"} 8192", metrics1);
            Assert.Contains("container_blk_bytes_total{container_name=\"bar\",direction=\"write\",container_label_label_pid=\"124\"} 8192", metrics2);
        }

        public void Dispose()
        {
            _api?.Dispose();
        }

        class TestContainerStateProvider : IContainerStateProvider
        {
            private int _pidIndex = 123;

            public Task<ContainerState[]> ProvideAsync()
            {
                var pid = _pidIndex++.ToString();

                return Task.FromResult(new[]
                {
                    new ContainerState
                    {
                        Pid = pid,
                        Name = "bar",
                        ContainerId = "foo",
                        Labels = new Dictionary<string, string>
                        {
                            {"label_pid", pid}
                        }
                    }
                });
            }
        }

        class TestFileContentProvider : IFileContentProviderV2
        {
            public Task<string> ReadNetStat(string containerPid)
            {
                return File.ReadAllTextAsync("files\\v2\\net.stat");
            }

            public Task<string> ReadCpuStat(string containerLongId)
            {
                return File.ReadAllTextAsync("files\\v2\\cpu.stat");
            }

            public Task<string> ReadIoStat(string containerLongId)
            {
                return File.ReadAllTextAsync("files\\v2\\io.stat");
            }

            public Task<string> ReadMemInfo()
            {
                return File.ReadAllTextAsync("files\\v2\\meminfo");
            }

            public Task<string> ReadMemStat(string containerLongId)
            {
                return File.ReadAllTextAsync("files\\v2\\memory.stat");
            }

            public Task<string> ReadMemSwapCurrent(string containerLongId)
            {
                return File.ReadAllTextAsync("files\\v2\\memory.swap.current");
            }

            public Task<string> ReadMemMax(string containerLongId)
            {
                return File.ReadAllTextAsync("files\\v2\\memory.max");
            }

            public Task<string> ReadSwapMax(string containerLongId)
            {
                return File.ReadAllTextAsync("files\\v2\\memory.swap.max");
            }
        }

        class TestCGroupDetector : ICGroupDetector
        {
            public Task<CGroupVersion> GetCGroupVersionAsync()
            {
                return Task.FromResult(CGroupVersion.V2);
            }
        }
    }
}
