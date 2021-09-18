using Microsoft.Extensions.DependencyInjection;

using System;
using System.Security.Claims;
using System.Threading;

namespace Brimborium.CodeFlow.RequestHandler {
    public abstract class RequestHandlerContextBase : IRequestHandlerContext {

        public RequestHandlerContextBase() {
        }
        public ContextId Id => this.GetId();

        protected abstract ContextId GetId();

        public abstract IRequestHandlerRootContextInternal GetRequestHandlerRootContext();

        public abstract IRequestHandlerContext CreateChild(ContextId id);

        public abstract IRequestHandlerFactory GetRequestHandlerFactory();

        public abstract TRequestHandler CreateRequestHandler<TRequestHandler>()
            where TRequestHandler : notnull, IRequestHandler;

    }

    public class RequestHandlerContext 
        : RequestHandlerContextBase
        , IRequestHandlerContext 
        , IRequestHandlerContextInternal {
        private readonly RequestHandlerContextBase _Parent;

        private readonly ContextId _Id;

        public RequestHandlerContext(RequestHandlerContextBase parent, ContextId id) {
            this._Parent = parent;
            this._Id = id;
        }

        protected override ContextId GetId() => this._Id;

        public override IRequestHandlerRootContextInternal GetRequestHandlerRootContext() => this._Parent.GetRequestHandlerRootContext();

        public override IRequestHandlerContext CreateChild(ContextId id) {
            return new RequestHandlerContext(this, id);
        }

        public override IRequestHandlerFactory GetRequestHandlerFactory() {
            return this.GetRequestHandlerRootContext().GetRequestHandlerFactory();
        }

        public override TRequestHandler CreateRequestHandler<TRequestHandler>() {
            TRequestHandler result = this.GetRequestHandlerRootContext().GetRequestHandlerFactory().CreateRequestHandler<TRequestHandler>();
            return result;
        }
    }

    public class RequestHandlerRootContext 
        : RequestHandlerContextBase
        , IRequestHandlerContextInternal
        , IRequestHandlerRootContext 
        , IRequestHandlerRootContextInternal {
        private IRequestHandlerFactory? _RequestHandlerFactory;
        private ClaimsPrincipal? _User;
        private Nullable<CancellationToken> _CancellationToken;

        public RequestHandlerRootContext(IServiceProvider scopedServiceProvider) {
            this.ScopedServiceProvider = scopedServiceProvider;
        }

        public IServiceProvider ScopedServiceProvider { get; }

        protected override ContextId GetId() => new ContextId(string.Empty, null);

        public override IRequestHandlerRootContextInternal GetRequestHandlerRootContext() => this;

        public override IRequestHandlerContext CreateChild(ContextId id) {
            return new RequestHandlerContext(this, id);
        }

        public IRequestHandlerFactory RequestHandlerFactory {
            get {
                return this.GetRequestHandlerFactory();
            }
            set {
                this._RequestHandlerFactory = value;
            }
        }

        public override IRequestHandlerFactory GetRequestHandlerFactory() {
            return _RequestHandlerFactory ??= this.ScopedServiceProvider.GetRequiredService<IRequestHandlerFactory>();
        }

        public override TRequestHandler CreateRequestHandler<TRequestHandler>() {
            return this.GetRequestHandlerFactory().CreateRequestHandler<TRequestHandler>();
        }

        public ClaimsPrincipal? GetUser() => this._User;
        public void SetUser(ClaimsPrincipal value) { this._User = value; }

        public CancellationToken? GetCancellationToken() => this._CancellationToken;
        public void SetCancellationToken(CancellationToken value) { this._CancellationToken = value; }
    }

    //public static class IRequestHandlerContextExtensions {
    //    public static IRequestHandlerContext CreateChildOfMethod<T>(
    //        this IRequestHandlerContext context,
    //        T callerInstance,
    //        [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
    //        )
    //        => ((RequestHandlerContextBase)context).CreateChild($"{typeof(T).FullName}.{memberName}");

    //    public static IRequestHandlerContext CreateChildOfTypeAndMethod<T>(
    //        this IRequestHandlerContext context,
    //        Type callerType,
    //        [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
    //        )
    //        => ((RequestHandlerContextBase)context).CreateChild($"{callerType.FullName}.{memberName}");
    //}
}
