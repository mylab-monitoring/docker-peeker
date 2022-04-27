using System.IO;
using System.Linq;
using MyLab.DockerPeeker.Tools;
using MyLab.DockerPeeker.Tools.StatObjectModel;
using Xunit;

namespace UnitTests
{
    public class MemInfoStatBehavior
    {
        [Fact]
        public void ShouldParse()
        {
            //Arrange
            var content = File.ReadAllText("files\\meminfo");

            //Act
            var stat = MemInfoStat.ParseMemInfo(content);

            //Assert
            Assert.Equal(51412888L * 1024, stat.MemTotal);
            Assert.Equal(1003516L * 1024, stat.SwapTotal);
        }
    }
}
