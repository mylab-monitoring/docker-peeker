using MyLab.DockerPeeker.Tools;
using Xunit;

namespace UnitTests
{
    public class ContainerMetricTypeBehavior
    {
        [Fact]
        public void ShouldCopy()
        {
            //Arrange
            var originType=  new ContainerMetricType
            {
                Name = "foo",
                Type = "bar",
                Description = "baz"
            };

            //Act
            var newType = new ContainerMetricType(originType);

            //Assert
            Assert.Equal(originType.Name, newType.Name);
            Assert.Equal(originType.Type, newType.Type);
            Assert.Equal(originType.Description, newType.Description);
            Assert.Equal(originType.Labels, newType.Labels);
        }

        [Fact]
        public void ShouldCopyWhenAddLabel()
        {
            //Arrange
            var originType = new ContainerMetricType
            {
                Name = "foo",
                Type = "bar",
                Description = "baz"
            };

            //Act
            var newType = originType.AddLabel("name", "val");

            //Assert
            Assert.Equal(originType.Name, newType.Name);
            Assert.Equal(originType.Type, newType.Type);
            Assert.Equal(originType.Description, newType.Description);
        }

        [Fact]
        public void ShouldAddLabel()
        {
            //Arrange
            var originType = new ContainerMetricType();

            //Act
            var newType = originType.AddLabel("foo", "bar", "baz");

            //Assert
            Assert.Single(newType.Labels);
            Assert.True(newType.Labels.ContainsKey("foo"));
            Assert.Equal("bar", newType.Labels["foo"]);
            Assert.Equal("baz", newType.Description);
        }
    }
}
