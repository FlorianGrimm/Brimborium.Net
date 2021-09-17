using System;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.CodeFlow.RequestHandler {
    public sealed class RequestHandlerContextBuilder : IRequestHandlerContextBuilder {
        private readonly IServiceProvider _ScopeServiceProvider;
        private IRequestHandlerRootContext? _RequestHandlerRootContext;

        public RequestHandlerContextBuilder(IServiceProvider scopeServiceProvider) {
            this._ScopeServiceProvider = scopeServiceProvider;
        }

        public IServiceProvider GetScopeServiceProvider() => this._ScopeServiceProvider;

        public bool TryGetRequestHandlerRootContext([MaybeNullWhen(false)] out IRequestHandlerRootContext context) {
            context = this._RequestHandlerRootContext;
            return (context is not null);
        }

        public void SetRequestHandlerContext(IRequestHandlerRootContext value) {
            this._RequestHandlerRootContext = value;
        }
    }
}
