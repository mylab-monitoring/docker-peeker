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
        
        [Fact]
        public void ShouldExtractNetParams()
        {
            //Arrange
            var str = "Inter-|   Receive                                                |  Transmit\r\n"
                + "  face |bytes    packets errs drop fifo frame compressed multicast|bytes    packets errs drop fifo colls carrier compressed\r\n"
                + "     lo: 33373754  320402    1    2    3     4          5         6 33373754  320402    7    8    9     1       2          3\r\n";
            var parser = StatParser.Create(str);

            //Act
            var parameters = parser.ExtractNetParams();

            //Assert
            Assert.Single(parameters);
            Assert.Equal("lo", parameters[0].Interface);
            Assert.Equal(33373754, parameters[0].ReceiveBytes);
            Assert.Equal(320402, parameters[0].ReceivePackets);
            Assert.Equal(1, parameters[0].ReceiveErrors);
            Assert.Equal(2, parameters[0].ReceiveDrop);
            Assert.Equal(3, parameters[0].ReceiveFifo);
            Assert.Equal(4, parameters[0].ReceiveFrame);
            Assert.Equal(5, parameters[0].ReceiveCompressed);
            Assert.Equal(6, parameters[0].ReceiveMulticast);
            Assert.Equal(33373754, parameters[0].TransmitBytes);
            Assert.Equal(320402, parameters[0].TransmitPackets);
            Assert.Equal(7, parameters[0].TransmitErrors);
            Assert.Equal(8, parameters[0].TransmitDrop);
            Assert.Equal(9, parameters[0].TransmitFifo);
            Assert.Equal(1, parameters[0].TransmitColls);
            Assert.Equal(2, parameters[0].TransmitCarrier);
            Assert.Equal(3, parameters[0].TransmitCompressed);

        }
    }
}
