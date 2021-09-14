using System;

namespace Brimborium.CodeFlow.RequestHandler {
    public interface IRequestHandlerFactory {
        TRequestHandler CreateRequestHandler<TRequestHandler>(
                IServiceProvider scopedServiceProvider
            )
            where TRequestHandler : notnull, IRequestHandler;
    }

    public interface ITypedRequestHandlerFactory<TRequestHandler>
        where TRequestHandler : notnull, IRequestHandler {
        TRequestHandler CreateTypedRequestHandler(
                IServiceProvider scopedServiceProvider
            );
    }
}
