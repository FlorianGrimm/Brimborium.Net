using System;

namespace Brimborium.CodeFlow.RequestHandler {
    public interface IGlobalRequestHandlerFactory {
        TRequestHandler CreateRequestHandler<TRequestHandler>(
                IServiceProvider scopedServiceProvider
            )
            where TRequestHandler : notnull, IRequestHandler;
    }
}
