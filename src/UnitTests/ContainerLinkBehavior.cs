using System;
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
            var createdAt = "2021-07-17 18:43:15 +0300 MSK";
            var stringLink = $"{id}\t{name}\t{createdAt}";

            //Act
            var link = ContainerLink.Read(stringLink);

            //Assert
            Assert.Equal(id, link.LongId);
            Assert.Equal(name, link.Name);
            Assert.Equal(new DateTime(2021, 07,17, 18, 43, 15, DateTimeKind.Utc), link.CreatedAt);
        }
    }
}
