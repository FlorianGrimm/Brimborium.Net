using System;

namespace Brimborium.CodeFlow.RequestHandler {
    public interface IGlobalRequestHandlerFactory {
        TRequestHandler CreateRequestHandler<TRequestHandler>(
                IServiceProvider scopedServiceProvider
            )
            where TRequestHandler : notnull, IRequestHandler;
    }

    public interface IScopeRequestHandlerFactory {
        TRequestHandler CreateRequestHandler<TRequestHandler>()
            where TRequestHandler : notnull, IRequestHandler;

        IRequestHandlerRootContext GetRequestHandlerRootContext();
    }

    public interface ITypedRequestHandlerFactory<TRequestHandler>
        where TRequestHandler : notnull, IRequestHandler {
        TRequestHandler CreateTypedRequestHandler(
                IServiceProvider scopedServiceProvider
            );
    }
}
