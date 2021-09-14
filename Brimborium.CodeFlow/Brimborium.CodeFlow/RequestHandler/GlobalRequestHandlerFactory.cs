using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Concurrent;

namespace Brimborium.CodeFlow.RequestHandler {
    public class GlobalRequestHandlerFactory : IGlobalRequestHandlerFactory {
        private ConcurrentDictionary<Type, bool> _TypedRequestHandlerExists;
        private ConcurrentDictionary<Type, object> _RequestHandlerFactoryByType;
        private readonly IServiceProvider _ServiceProvider;

        public GlobalRequestHandlerFactory(
            IServiceProvider serviceProvider
            ) {
            this._ServiceProvider = serviceProvider;
            this._RequestHandlerFactoryByType = new ConcurrentDictionary<Type, object>();
            this._TypedRequestHandlerExists = new ConcurrentDictionary<Type, bool>();
        }

        public TRequestHandler CreateRequestHandler<TRequestHandler>(
                IServiceProvider scopedServiceProvider
            )
            where TRequestHandler : notnull, IRequestHandler {

            var found = this._TypedRequestHandlerExists.TryGetValue(typeof(TRequestHandler), out bool exists);
            if (!found || (found && exists)) {
                if (this._RequestHandlerFactoryByType.TryGetValue(typeof(TRequestHandler), out var typedFactory)) {
                    var typedRequestHandlerFactory = (ITypedRequestHandlerFactory<TRequestHandler>)typedFactory;
                    return typedRequestHandlerFactory.CreateTypedRequestHandler(scopedServiceProvider);
                } else {
                    var typedRequestHandlerFactory = this._ServiceProvider.GetService<ITypedRequestHandlerFactory<TRequestHandler>>();
                    if (typedRequestHandlerFactory is not null) {
                        this._RequestHandlerFactoryByType.TryAdd(typeof(TRequestHandler), typedRequestHandlerFactory);
                        return typedRequestHandlerFactory.CreateTypedRequestHandler(scopedServiceProvider);
                    } else {
                        if (!found) {
                            this._TypedRequestHandlerExists.TryAdd(typeof(TRequestHandler), false);
                        }
                    }
                }
            }
            {
                var requestHandler = scopedServiceProvider.GetRequiredService<TRequestHandler>();
                return requestHandler;
            }
        }
    }

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
    }
}
