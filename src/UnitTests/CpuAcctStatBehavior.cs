using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLab.DockerPeeker.Tools;
using Xunit;

namespace UnitTests
{
    public class CpuAcctStatBehavior
    {
        [Fact]
        public void ShouldParse()
        {
            //Arrange
            var content = new StringBuilder()
                .AppendLine("user 46897")
                .AppendLine("system 60351")
                .ToString();

            //Act
            var stat = CpuAcctStat.Parse(content);

            //Assert
            Assert.Equal(46897, stat.User);
            Assert.Equal(60351, stat.System);
        }
    }
}
