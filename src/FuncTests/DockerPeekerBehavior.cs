using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient.Test;
using MyLab.DockerPeeker;
using MyLab.DockerPeeker.Services;
using MyLab.DockerPeeker.Tools;
using Xunit;
using Xunit.Abstractions;

namespace FuncTests
{
    public class DockerPeekerBehavior : IDisposable
    {
        private readonly TestApi<Startup, IMetricService> _api;

        public DockerPeekerBehavior(ITestOutputHelper output)
        {
            _api = new TestApi<Startup, IMetricService>
            {
                Output = output,
                ServiceOverrider = s => s
                    .AddSingleton<IFileContentProvider, TestFileContentProvider>()
                    .AddSingleton<IContainerListProvider, TestContainerListProvider>()
                    .AddSingleton<IContainerStateProvider, TestContainerStateProvider>()
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
            Assert.Contains("container_cpu_user_jiffies{name=\"bar\",container_label_label1=\"value1\"} 8313", metrics);
            Assert.Contains("container_cpu_system_jiffies{name=\"bar\",container_label_label1=\"value1\"} 10804",
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
            Assert.Contains("container_mem_swap_bytes{name=\"bar\",container_label_label1=\"value1\"} 0", metrics);
            Assert.Contains("container_mem_cache_bytes{name=\"bar\",container_label_label1=\"value1\"} 11492564992",
                metrics);
            Assert.Contains("container_mem_rss_bytes{name=\"bar\",container_label_label1=\"value1\"} 1930993664",
                metrics);
            Assert.Contains(
                "container_mem_limit_bytes{name=\"bar\",container_label_label1=\"value1\"} 9223372036854775807",
                metrics);
            Assert.Contains(
                "container_memsw_limit_bytes{name=\"bar\",container_label_label1=\"value1\"} 9223372036854775807",
                metrics);
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
            Assert.Contains("container_blk_read_bytes_total{name=\"bar\",container_label_label1=\"value1\"} 263622656", metrics);
            Assert.Contains("container_blk_writes_bytes{name=\"bar\",container_label_label1=\"value1\"} 0", metrics);
        }

        public void Dispose()
        {
            _api?.Dispose();
        }
    }

    class TestContainerStateProvider : IContainerStateProvider
    {
        public Task<ContainerState[]> ProvideAsync(string[] containersIds)
        {
            if (containersIds.Length != 1)
                throw new InvalidOperationException("Container id should be single");

            var containerLongId = containersIds.SingleOrDefault();

            if(string.IsNullOrWhiteSpace(containerLongId))
                throw new InvalidOperationException("Container id not defined");
            if(containerLongId != "foo")
                throw new InvalidOperationException("Met unexpected container id");

            return Task.FromResult(new []
            {
                new ContainerState("foo", "123", new Dictionary<string, string>()
                {
                    {"label1", "value1"}
                }), 
            });
        }
    }

    class TestContainerListProvider : IContainerListProvider
    {
        public Task<ContainerLink[]> ProviderActiveContainersAsync()
        {
            return Task.FromResult(new[]
            {
                new ContainerLink
                {
                    LongId = "foo",
                    Name = "bar"
                },
            });
        }
    }

    class TestFileContentProvider : IFileContentProvider
    {
        public Task<string> ReadCpuAcctStat(string containerLongId)
        {
            if (containerLongId != "foo")
                throw new InvalidOperationException("Met unexpected container id");

            var res = new StringBuilder()
                .AppendLine("user 8313")
                .AppendLine("system 10804")
                .ToString();

            return Task.FromResult(res);
        }

        public Task<string> ReadMemStat(string containerLongId)
        {
            if (containerLongId != "foo")
                throw new InvalidOperationException("Met unexpected container id");

            var res = new StringBuilder()
            .AppendLine("cache 11492564992")
            .AppendLine("rss 1930993664")
            .AppendLine("mapped_file 306728960")
            .AppendLine("pgpgin 406632648")
            .AppendLine("pgpgout 403355412")
            .AppendLine("swap 0")
            .AppendLine("pgfault 728281223")
            .AppendLine("pgmajfault 1724")
            .AppendLine("inactive_anon 46608384")
            .AppendLine("active_anon 1884520448")
            .AppendLine("inactive_file 7003344896")
            .AppendLine("active_file 4489052160")
            .AppendLine("unevictable 32768")
            .AppendLine("hierarchical_memory_limit 9223372036854775807")
            .AppendLine("hierarchical_memsw_limit 9223372036854775807")
            .AppendLine("total_cache 11492564992")
            .AppendLine("total_rss 1930993664")
            .AppendLine("total_mapped_file 306728960")
            .AppendLine("total_pgpgin 406632648")
            .AppendLine("total_pgpgout 403355412")
            .AppendLine("total_swap 0")
            .AppendLine("total_pgfault 728281223")
            .AppendLine("total_pgmajfault 1724")
            .AppendLine("total_inactive_anon 46608384")
            .AppendLine("total_active_anon 1884520448")
            .AppendLine("total_inactive_file 7003344896")
            .AppendLine("total_active_file 4489052160")
            .AppendLine("total_unevictable 32768")
                .ToString();

            return Task.FromResult(res);
        }

        public Task<string> ReadBlkIoServiceBytesStat(string containerLongId)
        {
            if (containerLongId != "foo")
                throw new InvalidOperationException("Met unexpected container id");

            var res = new StringBuilder()
            .AppendLine("253:1 Read 12599296")
            .AppendLine("253:1 Write 0")
            .AppendLine("253:1 Sync 0")
            .AppendLine("253:1 Async 12599296")
            .AppendLine("253:1 Total 12599296")
            .AppendLine("8:0 Read 131813376")
            .AppendLine("8:0 Write 0")
            .AppendLine("8:0 Sync 0")
            .AppendLine("8:0 Async 131813376")
            .AppendLine("8:0 Total 131813376")
            .AppendLine("253:0 Read 119209984")
            .AppendLine("253:0 Write 0")
            .AppendLine("253:0 Sync 0")
            .AppendLine("253:0 Async 119209984")
            .AppendLine("253:0 Total 119209984")
            .AppendLine("Total 263622656")
                .ToString();

            return Task.FromResult(res);
        }
    }
}
