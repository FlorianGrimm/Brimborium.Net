namespace Brimborium.CodeFlow.RequestHandler {
    public interface IScopeRequestHandlerFactory {
        TRequestHandler CreateRequestHandler<TRequestHandler>()
            where TRequestHandler : notnull, IRequestHandler;

        IRequestHandlerRootContext GetRequestHandlerRootContext();
    }
}
