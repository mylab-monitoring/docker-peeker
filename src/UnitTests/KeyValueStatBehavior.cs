using System.IO;
using MyLab.DockerPeeker.Tools.StatObjectModel;
using Xunit;

namespace UnitTests
{
    public class KeyValueStatBehavior
    {
        [Fact]
        public void ShouldParse()
        {
            //Arrange
            var content = File.ReadAllText("files\\v1\\cpuacct.stat");
            
            //Act
            var map = KeyValueStat.Parse(content);

            //Assert
            Assert.Equal(2, map.Count);
            Assert.True(map.ContainsKey("user"));
            Assert.Equal(8313, map["user"]);
            Assert.Equal(10804, map["system"]);

            Assert.True(map.ContainsKey("system"));
        }
    }
}