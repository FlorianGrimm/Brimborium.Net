namespace Brimborium.CodeFlow.RequestHandler {
    public interface IRequestHandlerContext { 
        IRequestHandlerContext CreateChild();
    }

    public interface IRequestHandlerRootContext : IRequestHandlerContext {
    }
}
