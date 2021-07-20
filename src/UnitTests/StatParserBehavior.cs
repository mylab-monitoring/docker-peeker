using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Tools;
using Xunit;

namespace UnitTests
{
    public class StatParserBehavior
    {
        [Fact]
        public void ShouldExtractPairParameter()
        {
            //Arrange
            var str = "foo 123\nbar 321";
            var parser = StatParser.Create(str);

            //Act
            var fooVal = parser.ExtractKey("foo", "foo param");

            //Assert
            Assert.Equal(123, fooVal);
        }

        [Fact]
        public void ShouldExtractIdentifiedParam()
        {
            //Arrange
            var str = "foo key1 123 \n foo key2 456 \n bar key1 789";
            var parser = StatParser.Create(str);

            //Act
            var parameters = parser.ExtractIdentifiable("foo", "foo param");

            //Assert
            Assert.Equal(2, parameters.Length);
            Assert.Equal("foo", parameters[0].Id);
            Assert.Equal("key1", parameters[0].Key);
            Assert.Equal(123, parameters[0].Value);
            Assert.Equal("foo", parameters[1].Id);
            Assert.Equal("key2", parameters[1].Key);
            Assert.Equal(456, parameters[1].Value);
        }

        [Fact]
        public void ShouldExtractAllIdentifiedParam()
        {
            //Arrange
            var str = "foo key1 123 \n bar key1 789";
            var parser = StatParser.Create(str);

            //Act
            var parameters = parser.ExtractIdentifiable();

            //Assert
            Assert.Equal(2, parameters.Length);
            Assert.Equal("foo", parameters[0].Id);
            Assert.Equal("key1", parameters[0].Key);
            Assert.Equal(123, parameters[0].Value);
            Assert.Equal("bar", parameters[1].Id);
            Assert.Equal("key1", parameters[1].Key);
            Assert.Equal(789, parameters[1].Value);
        }
    }
}
