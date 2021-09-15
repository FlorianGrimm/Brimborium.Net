using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Brimborium.CodeFlow.RequestHandler {
    public sealed class RequestHandlerContextHolder : IRequestHandlerContextHolder {
        private readonly IServiceProvider _ScopeServiceProvider;
        private IRequestHandlerContext? _RequestHandlerContext;

        public RequestHandlerContextHolder(IServiceProvider scopeServiceProvider) {
            this._ScopeServiceProvider = scopeServiceProvider;
        }

        public IServiceProvider GetScopeServiceProvider() => this._ScopeServiceProvider;

        public bool TryGetRequestHandlerContext(bool createIfNeeded, [MaybeNullWhen(false)] out IRequestHandlerContext context) {
            if (createIfNeeded) {
                context = (this._RequestHandlerContext ??= this._ScopeServiceProvider.GetRequiredService<IRequestHandlerRootContext>());
                return true;
            } else {
                context = this._RequestHandlerContext;
                return (context is not null);
            }
        }

        public void SetRequestHandlerContext(IRequestHandlerContext value) {
            this._RequestHandlerContext = value;
        }
    }
}
