using System.Linq;
using MyLab.DockerPeeker.Tools;
using Xunit;

namespace UnitTests
{
    public class DockerStatParserBehavior
    {
        [Fact]
        public void ShouldParse()
        {
            //Arrange
            const string testStr = "cd27c887b2c09e4331d93513cb088f09f3d98a76ae4896366eda574cef926882\tsonar_sonarqube_1\t4.11%\t2.063GiB / 12.43GiB\t16.60%\t0B / 0B\t1.59kB / 0B\nd4380a2d1bc9e7c31e75587e2a4c73f7d97abfe1f45f10ea20469415041edbd5\topenresty_openresty_1\t0.00%\t5.668MiB / 12.43GiB\t0.04%\t0B / 0B\t1.84kB / 154B\n947fb46a483118f6f78f9c8572ed18f1f93ec938a80880b5f018eb6c572513bf\tMyLab.DockerPeeker\t1.30%\t117.7MiB / 12.43GiB\t0.92%\t0B / 0B\t22.7kB / 171kB\n";

            //Act
            var res = DockerStatParser.Parse(testStr).ToArray();

            //Assert
            Assert.Equal(3, res.Length);
            Assert.Equal("sonar_sonarqube_1", res[0].ContainerName);
            Assert.Equal("openresty_openresty_1", res[1].ContainerName);
            Assert.Equal("MyLab.DockerPeeker", res[2].ContainerName);
        }
    }
}
