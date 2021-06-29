using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;
using MyLab.DockerPeeker.Tools;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class MetricsBuilderBehavior
    {
        private readonly ITestOutputHelper _output;

        public MetricsBuilderBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task ShouldBuild()
        {
            //Arrange
            var labelsProvider = new TestContainerLabelsProvider();
            var mBuilder = new MetricsBuilder(labelsProvider);
            var sb = new StringBuilder();
            var stat = new[]
            {
                new DockerStatItem
                {
                    ContainerId = "foo",
                    HostCpuUsage = 1.1,
                    HostMemUsage = 2.2,
                    ContainerMemUsage = 3.3,
                    ContainerMemLimit = 4.4,
                    BlockRx = 5.5,
                    BlockTx = 6.6,
                    NetRx = 7.7,
                    NetTx = 8.8,
                    ContainerName = "foo"
                },
            };

            //Act
            await mBuilder.Build(sb, stat);
            var strOutput = sb.ToString();

            _output.WriteLine(strOutput);

            //Assert
            Assert.Contains("container_host_cpu_usage_percentages_total{name=\"foo\",container_label_bar=\"baz\"} 1.10", strOutput);
            Assert.Contains("container_host_memory_usage_percentages_total{name=\"foo\",container_label_bar=\"baz\"} 2.20", strOutput);
            Assert.Contains("container_memory_usage_bytes_total{name=\"foo\",container_label_bar=\"baz\"} 3.30", strOutput);
            Assert.Contains("container_memory_limit_bytes_total{name=\"foo\",container_label_bar=\"baz\"} 4.40", strOutput);
            Assert.Contains("container_block_input_bytes_total{name=\"foo\",container_label_bar=\"baz\"} 5.50", strOutput);
            Assert.Contains("container_block_output_bytes_total{name=\"foo\",container_label_bar=\"baz\"} 6.60", strOutput);
            Assert.Contains("container_network_input_bytes_total{name=\"foo\",container_label_bar=\"baz\"} 7.70", strOutput);
            Assert.Contains("container_network_output_bytes_total{name=\"foo\",container_label_bar=\"baz\"} 8.80", strOutput);
        }

        class TestContainerLabelsProvider : IContainerLabelsProvider
        {
            public Task<ContainerLabels[]> Provide(string[] containersIds)
            {
                return Task.FromResult(new []
                {
                    new ContainerLabels("foo", new Dictionary<string, string>
                    {
                        {"bar", "baz"}
                    }),
                });
            }
        }
    }
}
