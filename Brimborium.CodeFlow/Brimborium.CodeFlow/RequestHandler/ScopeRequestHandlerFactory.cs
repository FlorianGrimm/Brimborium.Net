using Microsoft.Extensions.DependencyInjection;

using System;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.RequestHandler {
    public class ScopeRequestHandlerFactory : IScopeRequestHandlerFactory {
        private readonly IGlobalRequestHandlerFactory _GlobalRequestHandlerFactory;
        private readonly IServiceProvider _ScopedServiceProvider;

        public ScopeRequestHandlerFactory(
            IGlobalRequestHandlerFactory globalRequestHandlerFactory,
            IServiceProvider scopedServiceProvider
            ) {
            this._GlobalRequestHandlerFactory = globalRequestHandlerFactory;
            this._ScopedServiceProvider = scopedServiceProvider;
        }

        public TRequestHandler CreateRequestHandler<TRequestHandler>()
            where TRequestHandler : notnull, IRequestHandler
            => this._GlobalRequestHandlerFactory.CreateRequestHandler<TRequestHandler>(this._ScopedServiceProvider);

        public IRequestHandlerRootContext GetRequestHandlerRootContext()
            => this._ScopedServiceProvider.GetRequiredService<IRequestHandlerRootContext>();

        public Func<TRequest, Task<TResponse>> CreateRequestHandlerFunc<TRequestHandler, TRequest, TResponse>(
            IRequestHandlerContext context
            ) where TRequestHandler : notnull, IRequestHandler, IRequestHandler<TRequest, TResponse> {
            var requestHandler = this.CreateRequestHandler<TRequestHandler>();
            var requestHandlerBound = new RequestHandlerClosure<TRequestHandler, TRequest, TResponse>(requestHandler, context);
            return requestHandlerBound.ExecuteAsync;
        }
    }
}
