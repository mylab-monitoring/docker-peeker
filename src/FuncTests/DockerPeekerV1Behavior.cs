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
    public class DockerPeekerV1Behavior : IDisposable
    {
        private readonly TestApi<Startup, IMetricService> _api;

        public DockerPeekerV1Behavior(ITestOutputHelper output)
        {
            _api = new TestApi<Startup, IMetricService>
            {
                Output = output,
                ServiceOverrider = s => s
                    .AddSingleton<IFileContentProviderV1, TestFileContentProvider>()
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
            Assert.Contains("container_cpu_jiffies_total{container_name=\"bar\",mode=\"user\",container_label_label_pid=\"123\"} 8313", metrics);
            Assert.Contains("container_cpu_jiffies_total{container_name=\"bar\",mode=\"system\",container_label_label_pid=\"123\"} 10804",
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
            Assert.Contains("container_mem_bytes{container_name=\"bar\",type=\"swap\",container_label_label_pid=\"123\"} 0", metrics);
            Assert.Contains("container_mem_bytes{container_name=\"bar\",type=\"cache\",container_label_label_pid=\"123\"} 11492564992", metrics);
            Assert.Contains("container_mem_bytes{container_name=\"bar\",type=\"rss\",container_label_label_pid=\"123\"} 1930993664", metrics);
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
            Assert.Contains("container_blk_bytes_total{container_name=\"bar\",direction=\"read\",container_label_label_pid=\"123\"} 263622656", metrics);
            Assert.Contains("container_blk_bytes_total{container_name=\"bar\",direction=\"write\",container_label_label_pid=\"123\"} 0", metrics);
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
            Assert.Contains("container_blk_bytes_total{container_name=\"bar\",direction=\"write\",container_label_label_pid=\"123\"} 0", metrics1);
            Assert.Contains("container_blk_bytes_total{container_name=\"bar\",direction=\"write\",container_label_label_pid=\"124\"} 0", metrics2);
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
                        Id = "foo",
                        IsActive = true,
                        Labels = new Dictionary<string, string>
                        {
                            {"label_pid", pid}
                        }
                    }
                });
            }
        }

        class TestFileContentProvider : IFileContentProviderV1
        {
            public Task<string> ReadCpuStat(string containerLongId)
            {
                return File.ReadAllTextAsync("files\\v1\\cpuacct.stat");
            }

            public Task<string> ReadMemStat(string containerLongId)
            {
                return File.ReadAllTextAsync("files\\v1\\memory.stat");
            }

            public Task<string> ReadBlkStat(string containerLongId)
            {
                return File.ReadAllTextAsync("files\\v1\\blkio.throttle.io_service_bytes");
            }

            public Task<string> ReadNetStat(string containerPid)
            {
                return File.ReadAllTextAsync("files\\v1\\netstat.txt");
            }
        }

        class TestCGroupDetector : ICGroupDetector
        {
            public Task<CGroupVersion> GetCGroupVersionAsync()
            {
                return Task.FromResult(CGroupVersion.V1);
            }
        }
    }
}
