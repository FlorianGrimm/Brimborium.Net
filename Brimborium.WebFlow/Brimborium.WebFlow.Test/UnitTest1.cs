using System;
using Xunit;

namespace Brimborium.WebFlow.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var x = ^1;
            Assert.Equal(0, ^1);
            Assert.Equal(1, ^1);
        }
    }
}
