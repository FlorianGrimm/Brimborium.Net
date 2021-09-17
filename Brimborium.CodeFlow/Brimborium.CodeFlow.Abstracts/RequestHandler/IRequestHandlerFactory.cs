namespace Brimborium.CodeFlow.RequestHandler {
    public interface IRequestHandlerFactory {
        TRequestHandler CreateRequestHandler<TRequestHandler>()
            where TRequestHandler : notnull, IRequestHandler;
    }
}
