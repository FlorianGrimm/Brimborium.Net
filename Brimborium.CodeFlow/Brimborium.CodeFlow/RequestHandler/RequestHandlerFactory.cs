using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Brimborium.CodeFlow.RequestHandler {
    public class RequestHandlerFactory : IRequestHandlerFactory {
        private static Dictionary<Type, bool> _TypedRequestHandlerExists = new Dictionary<Type, bool>();
        private Dictionary<Type, IRequestHandler> _RequestHandlerByType;
        private ConcurrentDictionary<Type, object> _RequestHandlerFactoryByType;
        private readonly IServiceProvider _ServiceProvider;

        public RequestHandlerFactory(
            IServiceProvider serviceProvider
            ) {
            this._ServiceProvider = serviceProvider;
            this._RequestHandlerByType = new Dictionary<Type, IRequestHandler>();
            this._RequestHandlerFactoryByType = new ConcurrentDictionary<Type, object>();
        }

        public TRequestHandler CreateRequestHandler<TRequestHandler>(
                IServiceProvider scopedServiceProvider
            )
            where TRequestHandler : notnull, IRequestHandler {

            if (this._RequestHandlerFactoryByType.TryGetValue(typeof(TRequestHandler), out var typedFactory)) {
                var typedRequestHandlerFactory = (ITypedRequestHandlerFactory<TRequestHandler>)typedFactory;
                return typedRequestHandlerFactory.CreateTypedRequestHandler(scopedServiceProvider);
            } else {
                var typedRequestHandlerFactory = this._ServiceProvider.GetService<ITypedRequestHandlerFactory<TRequestHandler>>();
                if (typedRequestHandlerFactory is not null) {
                    this._RequestHandlerFactoryByType.TryAdd(typeof(TRequestHandler), typedRequestHandlerFactory);
                    return typedRequestHandlerFactory.CreateTypedRequestHandler(scopedServiceProvider);
                }
            }
            {
                var requestHandler = scopedServiceProvider.GetRequiredService<TRequestHandler>();
                return requestHandler;
            }
        }
    }
}
