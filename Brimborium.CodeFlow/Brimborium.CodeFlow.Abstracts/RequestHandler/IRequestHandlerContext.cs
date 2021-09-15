using System;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.CodeFlow.RequestHandler {
    public interface IRequestHandlerContext {
        ContextId Id { get; }

        IRequestHandlerContext CreateChild(ContextId id);

        IScopeRequestHandlerFactory GetRequestHandlerFactory();
    }

    public interface IRequestHandlerRootContext : IRequestHandlerContext {
    }

    public interface IRequestHandlerContextHolder {
        IServiceProvider GetScopeServiceProvider();
        bool TryGetRequestHandlerContext(bool createIfNeeded, [MaybeNullWhen(false)] out IRequestHandlerContext context);
        void SetRequestHandlerContext(IRequestHandlerContext value);
    }

    public record ContextId(string Id, ContextId? ParentId) {
        public static implicit operator ContextId(string id) {
            return new ContextId(id);
        }
    }
}
