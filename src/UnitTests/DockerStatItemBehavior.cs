using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyLab.DockerPeeker.Tools;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class DockerStatItemBehavior
    {
        private readonly ITestOutputHelper _output;

        public DockerStatItemBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldParse()
        {
            //Arrange
            var testInput = "947fb46a\tMyLab.DockerPeeker\t23.41%\t9.84MiB / 12.43GiB\t0.08%\t10B / 20B\t16.1kB / 163kB\n";

            //Act
            var itm = DockerStatItem.Parse(testInput);

            //Assert
            Assert.NotNull(itm);
            Assert.Equal("947fb46a", itm.ContainerId);
            Assert.Equal(23.41d, itm.HostCpuUsage);
            Assert.Equal(0.08d, itm.HostMemUsage);
            Assert.Equal(1024 * 1024 * 9.84d, itm.ContainerMemUsage);
            Assert.Equal(1024 * 1024 * 1024 * 12.43d, itm.ContainerMemLimit);
            Assert.Equal(10d, itm.BlockInput);
            Assert.Equal(20d, itm.BlockOutput);
            Assert.Equal(1024 * 16.1d, itm.NetInput);
            Assert.Equal(1024 * 163d, itm.NetOutput);
        }

        [Fact]
        public void ShouldNotParseWithWrongCountOfParameters()
        {
            //Arrange
            var testInput = "MyLab.DockerPeeker\t23.41%\t9.84MiB / 12.43GiB\t0.08%\t10B / 20B\n";

            //Act && Assert
            var ex = Assert.Throws<FormatException>(() => DockerStatItem.Parse(testInput));

            _output.WriteLine(ex.Message);
        }

        [Theory]
        [InlineData("ololo")]
        [InlineData("947fb46a\tMyLab.DockerPeeker\t23-41%\t9.84MiB / 12.43GiB\t0.08%\t10B / 20B\t16.1kB / 163kB\n")]
        [InlineData("947fb46a\tMyLab.DockerPeeker\t23.41%\t9.84MiB - 12.43GiB\t0.08%\t10B / 20B\t16.1kB / 163kB\n")]
        [InlineData("947fb46a\tMyLab.DockerPeeker\t23.41%\t9-84MiB / 12.43GiB\t0.08%\t10B / 20B\t16.1kB / 163kB\n")]
        [InlineData("947fb46a\tMyLab.DockerPeeker\t23.41%\t9.84MiB / 12-43GiB\t0.08%\t10B / 20B\t16.1kB / 163kB\n")]
        [InlineData("947fb46a\tMyLab.DockerPeeker\t23.41%\t9.84MiB / 12.43\t0.08%\t10B / 20B\t16.1kB / 163kB\n")]
        public void ShouldNotParseWithWrongFormatted(string testInput)
        {
            //Act && Assert
            var ex= Assert.Throws<FormatException>(() => DockerStatItem.Parse(testInput));

            _output.WriteLine(ex.Message);
        }

        [Theory]
        [MemberData(nameof(GetRealMetrics))]
        //[InlineData("45dbf20cb2edc93b7f4d9e35912d63f515ddd5f7a29e6e4c94f4dac7e592f9ce\tinfonot-get-avatar\t0.00%\t728KiB / 7.795GiB\t0.01%\t39.7MB / 0B\t218MB / 187MB")]
        public void ShouldParseRealMetrics(string testInput)
        {
            //Act && Assert
            DockerStatItem.Parse(testInput);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void ShouldNotParseEmptyString(string input)
        {
            //Act && Assert
            var ex = Assert.Throws<ArgumentException>(() => DockerStatItem.Parse(input));

            _output.WriteLine(ex.Message);
        }

        public static IEnumerable<object[]> GetRealMetrics()
        {
            return File.ReadAllLines("real-metrics.txt")
                .Select(s => new object[] {s});
        }
    }
}
