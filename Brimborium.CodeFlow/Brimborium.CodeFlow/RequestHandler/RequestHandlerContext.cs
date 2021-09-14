using Microsoft.Extensions.DependencyInjection;

using System;

namespace Brimborium.CodeFlow.RequestHandler {
    // TODO
    public class RequestHandlerContext : IRequestHandlerContext {
    }

    public class RequestHandlerRootContext : IRequestHandlerRootContext {
        private IRequestHandlerFactory? _RequestHandlerFactory;
        public RequestHandlerRootContext(IServiceProvider scopedServiceProvider) {
            this.ScopedServiceProvider = scopedServiceProvider;
        }

        public IServiceProvider ScopedServiceProvider { get; }

        public IRequestHandlerFactory GetRequestHandlerFactory() {
            return _RequestHandlerFactory ??= this.ScopedServiceProvider.GetRequiredService<IRequestHandlerFactory>();
        }
    }
}
