using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Brimborium.CodeFlow.RequestHandler {
    public class RequestHandlerRootContextTest {
        [Fact]
        public void RequestHandlerRootContext_01_Test() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRequestHandler();
            var globalSP = services.BuildServiceProvider();
            using var scope = globalSP.CreateScope();
            var ctxt = scope.ServiceProvider.GetRequiredService<IRequestHandlerRootContext>();
            Assert.True(ctxt is RequestHandlerRootContext);
            Assert.NotNull(((RequestHandlerRootContext)ctxt).ScopedServiceProvider);
            abc(1, ctxt);
        }
        private static void abc(int a, IRequestHandlerContext ctxt) {
            ctxt = ctxt.CreateChild();
            //ctxt.LogParameter(nameof(a), a);
        }
    }
}
