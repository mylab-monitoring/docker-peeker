using System.IO;
using MyLab.DockerPeeker.Tools.StatObjectModel;
using Xunit;

namespace UnitTests
{
    public class NetStatBehavior
    {
        [Fact]
        public void ShouldParse()
        {
            //Arrange
            var content = File.ReadAllText("files\\netstat.txt");

            //Act
            var stat = NetStat.Parse(content);

            //Assert
            Assert.Equal(2, stat.Count);

            Assert.True(stat.ContainsKey("lo"));
            Assert.Equal(33373754, stat["lo"].ReceiveBytes);
            Assert.Equal(33373755, stat["lo"].TransmitBytes);

            Assert.True(stat.ContainsKey("eth3"));
            Assert.Equal(10, stat["eth3"].ReceiveBytes);
            Assert.Equal(20, stat["eth3"].TransmitBytes);
        }
    }
}