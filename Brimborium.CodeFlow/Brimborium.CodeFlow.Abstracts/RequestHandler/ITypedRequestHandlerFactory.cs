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

    //public interface ITypedRequestHandlerFactory<TRequest, TResponse, TRequestHandler>
    //    : ITypedRequestHandlerFactory
    //    where TRequestHandler : notnull, IRequestHandler, IRequestHandler<TRequest, TResponse, TRequestHandler> {
    //    TRequestHandler CreateTypedRequestHandler(
    //            IServiceProvider scopedServiceProvider,
    //            IRequestHandlerContext context
    //        );
    //}
}
