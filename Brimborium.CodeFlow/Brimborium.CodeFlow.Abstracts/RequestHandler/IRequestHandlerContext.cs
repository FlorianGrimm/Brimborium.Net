﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading;

namespace Brimborium.CodeFlow.RequestHandler {
    public interface IRequestHandlerContext {
        ContextId Id { get; }

        IRequestHandlerContext CreateChild(ContextId id);

        IRequestHandlerFactory GetRequestHandlerFactory();

        TRequestHandler CreateRequestHandler<TRequestHandler>()
            where TRequestHandler : notnull, IRequestHandler;
    }

    public interface IRequestHandlerContextInternal : IRequestHandlerContext { 
        IRequestHandlerRootContextInternal GetRequestHandlerRootContext();
    }

    public interface IRequestHandlerRootContext : IRequestHandlerContext {
    }

    public interface IRequestHandlerRootContextInternal : IRequestHandlerRootContext {
        ClaimsPrincipal? GetUser();
        void SetUser(ClaimsPrincipal value);

        CancellationToken? GetCancellationToken();
        void SetCancellationToken(CancellationToken value);
    }

    //public interface IRequestHandlerContext<TRequest, TResponse, TRequestHandler>
    //    where TRequestHandler : IRequestHandler<TRequest, TResponse, TRequestHandler> {
    //    Task<TResponse> CallRequestHandlerAsync(TRequest request);
    //}

    public interface IRequestHandlerSupport {
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
