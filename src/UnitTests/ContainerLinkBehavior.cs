using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Services;
using Xunit;

namespace UnitTests
{
    public class ContainerLinkBehavior
    {
        [Fact]
        public void ShouldParse()
        {
            //Arrange
            var id = "58b7663bc530bef06e679f79334bcb3cce051806e6907f00f340e5bb703f6a64";
            var name = "foo";
            var stringLink = $"{id}\t{name}";

            //Act
            var link = ContainerLink.Read(stringLink);

            //Assert
            Assert.Equal(id, link.LongId);
            Assert.Equal(name, link.Name);
        }
    }
}
