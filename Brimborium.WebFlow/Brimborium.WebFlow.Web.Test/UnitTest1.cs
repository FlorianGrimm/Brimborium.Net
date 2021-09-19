
using System;

using Xunit;

namespace Brimborium.WebFlow.Web.Test {
    public class UnitTest1 : IClassFixture<CustomWebApplicationFactory<Startup>> {
        private readonly CustomWebApplicationFactory<Startup> _Factory;

        public UnitTest1(CustomWebApplicationFactory<Startup> factory) {
            this._Factory = factory;
        }

        [Fact]
        public void Test1() {
            // this._Factory.CreateClient
        }
    }
}
