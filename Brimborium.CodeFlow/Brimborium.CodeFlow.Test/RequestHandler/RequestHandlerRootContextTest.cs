using Brimborium.CodeFlow.FluentIL;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace Brimborium.CodeFlow.RequestHandler {
    public class RequestHandlerRootContextTest {
        [Fact]
        public void RequestHandlerRootContext_01_Test() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRequestHandler();
            services.AddScoped<GnaController>();
            var globalSP = services.BuildServiceProvider();
            using var scope = globalSP.CreateScope();
            var scopeServiceProvider = scope.ServiceProvider;
            var ctxt = scopeServiceProvider.GetRequiredService<IRequestHandlerRootContext>();

            Assert.True(ctxt is RequestHandlerRootContext);
            Assert.NotNull(((RequestHandlerRootContext)ctxt).ScopedServiceProvider);
            abc(1, ctxt);
        }

        [Fact]
        public async Task RequestHandlerRootContext_02_TestAsync() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRequestHandler();
            services.AddScoped<GnaController>();
            var globalSP = services.BuildServiceProvider();
            using var scope = globalSP.CreateScope();
            var scopeServiceProvider = scope.ServiceProvider;
            var gnaController = scopeServiceProvider.GetRequiredService<GnaController>();
            await gnaController.PostAsync(1);
            var ctxt = scopeServiceProvider.GetRequiredService<IRequestHandlerRootContext>();

            Assert.True(ctxt is RequestHandlerRootContext);
            Assert.NotNull(((RequestHandlerRootContext)ctxt).ScopedServiceProvider);
            abc(1, ctxt);
        }

        private static void abc(int a, IRequestHandlerContext ctxt) {
            ctxt = ctxt.CreateChild();
            //ctxt.LogParameter(nameof(a), a);
        }

        public record GnaReq {
            public int Value { get; set; }
        }

        public record GnaResp {
            public int Value { get; set; }
        }

        public class GnaController {
            private readonly IScopeRequestHandlerFactory _RequestHandlerFactory;

            public GnaController(
                IScopeRequestHandlerFactory requestHandlerFactory) {
                this._RequestHandlerFactory = requestHandlerFactory;
            }

            public async Task<int> PostAsync(int value) {
                await Task.CompletedTask;
                var request = new GnaReq() { Value = value };
                var context = this._RequestHandlerFactory.GetRequestHandlerRootContext();
                var requestHandler = this._RequestHandlerFactory.CreateRequestHandler<IGnaRequestHandler>();
                var response = await requestHandler.ExecuteAsync(request, context);
                return response.Value;
            }
        }

        public interface IGnaRequestHandler : IRequestHandler<GnaReq, GnaResp> { }

        public interface IGnaTypedRequestHandlerFactory : ITypedRequestHandlerFactory<IGnaRequestHandler> { }

        public class GnaTypedRequestHandlerFactory : IGnaTypedRequestHandlerFactory {
            public GnaTypedRequestHandlerFactory() { }
            public IGnaRequestHandler CreateTypedRequestHandler(IServiceProvider scopedServiceProvider) {
                return scopedServiceProvider.GetRequiredService<GnaRequestHandler>();
            }
        }

        public class GnaRequestHandler : IGnaRequestHandler {
            public GnaRequestHandler() { }
            public async Task<GnaResp> ExecuteAsync(GnaReq request, IRequestHandlerContext context, CancellationToken cancellationToken = default) {
                await Task.CompletedTask;
                var result = new GnaResp() { Value = request.Value };
                return result;
            }
        }
    }
}
