using System.IO;
using MyLab.DockerPeeker.Tools.StatObjectModel;
using Xunit;

namespace UnitTests
{
    public class BlkIoStatBehavior
    {
        [Fact]
        public void ShouldParseV1()
        {
            //Arrange
            var content = File.ReadAllText("files\\v1\\blkio.throttle.io_service_bytes");

            //Act
            var stat = BlkIoStat.ParseV1(content);

            //Assert
            Assert.Equal(3, stat.Count);

            Assert.True(stat.ContainsKey("253:0"));
            Assert.Equal(119209984, stat["253:0"].Read);
            Assert.Equal(0, stat["253:0"].Write);

            Assert.True(stat.ContainsKey("253:1"));
            Assert.Equal(12599296, stat["253:1"].Read);
            Assert.Equal(0, stat["253:1"].Write);

            Assert.True(stat.ContainsKey("8:0"));
            Assert.Equal(131813376, stat["8:0"].Read);
            Assert.Equal(123, stat["8:0"].Write);
        }

        [Fact]
        public void ShouldParseV2()
        {
            //Arrange
            var content = File.ReadAllText("files\\v2\\io.stat");

            //Act
            var stat = BlkIoStat.ParseV2(content);

            //Assert
            Assert.Equal(2, stat.Count);

            Assert.True(stat.ContainsKey("254:0"));
            Assert.Equal(509908, stat["254:0"].Read);
            Assert.Equal(4336, stat["254:0"].Write);
            
            Assert.True(stat.ContainsKey("8:0"));
            Assert.Equal(503808, stat["8:0"].Read);
            Assert.Equal(4096, stat["8:0"].Write);
        }
    }
}