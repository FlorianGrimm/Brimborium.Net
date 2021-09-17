using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.RequestHandler {
    public interface IRequestHandlerContext : IDisposable {
        ContextId Id { get; }

        IRequestHandlerContext CreateChild(ContextId id);

        IRequestHandlerFactory GetRequestHandlerFactory();

        TRequestHandler CreateRequestHandler<TRequestHandler>()
            where TRequestHandler : notnull, IRequestHandler;
    }

    public interface IRequestHandlerRootContext : IRequestHandlerContext {
    }

    //public interface IRequestHandlerContext<TRequest, TResponse, TRequestHandler>
    //    where TRequestHandler : IRequestHandler<TRequest, TResponse, TRequestHandler> {
    //    Task<TResponse> CallRequestHandlerAsync(TRequest request);
    //}

    public interface IRequestHandlerContextBuilder {
        IServiceProvider GetScopeServiceProvider();
        bool TryGetRequestHandlerRootContext([MaybeNullWhen(false)] out IRequestHandlerRootContext context);
        void SetRequestHandlerContext(IRequestHandlerRootContext value);
    }

    public record ContextId(string Id, ContextId? ParentId) {
        public static implicit operator ContextId(string id) {
            return new ContextId(id);
        }
    }
}
