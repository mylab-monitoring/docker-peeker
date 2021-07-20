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
                    .AddSingleton<IContainerLabelsProvider, TestContainerLabelsProvider>()
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
        public async Task ShouldProvideCpuAcct()
        {
            //Arrange
            var client = _api.StartWithProxy();

            //Act
            var metrics = (await client.GetMetrics())
                .Split('\n')
                .Select(s => s.Trim())
                .ToArray();

            //Assert
            Assert.Contains("container_cpu_user_jiffies_total{name=\"bar\",container_label_label1=\"value1\"} 8313.00", metrics);
            Assert.Contains("container_cpu_system_jiffies_total{name=\"bar\",container_label_label1=\"value1\"} 10804.00", metrics);
        }

        public void Dispose()
        {
            _api?.Dispose();
        }
    }

    class TestContainerLabelsProvider : IContainerLabelsProvider
    {
        public Task<ContainerLabels[]> ProvideAsync(string[] containersIds)
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
                new ContainerLabels("foo", new Dictionary<string, string>()
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
        public Task<string> ReadCpuAcct(string containerLongId)
        {
            if(containerLongId != "foo")
                throw new InvalidOperationException("Met unexpected container id");

            var res = new StringBuilder()
                .AppendLine("user 8313")
                .AppendLine("system 10804")
                .ToString();

            return Task.FromResult(res);
        }
    }
}
