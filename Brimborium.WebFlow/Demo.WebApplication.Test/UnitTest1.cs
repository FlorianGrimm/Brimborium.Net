using System;

using Xunit;

namespace Demo.WebApplication {
    public class UnitTest1 {
        private readonly CustomWebApplicationFactory<Startup> _Factory;

        public UnitTest1(CustomWebApplicationFactory<Startup> factory) {
            this._Factory = factory;
        }

        [Fact]
        public void Test1() {
#warning here
            // this._Factory.CreateClient
        }
    }
}
