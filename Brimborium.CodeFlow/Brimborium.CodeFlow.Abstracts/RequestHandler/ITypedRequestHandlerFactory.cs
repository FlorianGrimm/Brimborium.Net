using System;

namespace Brimborium.CodeFlow.RequestHandler {
    public interface ITypedRequestHandlerFactory { 
    }
    
    public interface ITypedRequestHandlerFactory<TRequestHandler>
        : ITypedRequestHandlerFactory
        where TRequestHandler : notnull, IRequestHandler {
        TRequestHandler CreateTypedRequestHandler(
                IServiceProvider scopedServiceProvider
            );
    }
}
