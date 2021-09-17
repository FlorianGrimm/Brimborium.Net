using Microsoft.Extensions.DependencyInjection;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.RequestHandler {
    public abstract class RequestHandlerContextBase : IRequestHandlerContext {

        public RequestHandlerContextBase() {
        }
        public ContextId Id => this.GetId();

        protected abstract ContextId GetId();

        protected internal abstract RequestHandlerRootContext GetRoot();


        public abstract IRequestHandlerContext CreateChild(ContextId id);

        public abstract IRequestHandlerFactory GetRequestHandlerFactory();

        public abstract TRequestHandler CreateRequestHandler<TRequestHandler>()
            where TRequestHandler : notnull, IRequestHandler;

    }

    public class RequestHandlerContext : RequestHandlerContextBase, IRequestHandlerContext {
        private readonly RequestHandlerContextBase _Parent;

        private readonly ContextId _Id;

        public RequestHandlerContext(RequestHandlerContextBase parent, ContextId id) {
            this._Parent = parent;
            this._Id = id;
        }

        protected override ContextId GetId() => this._Id;

        protected internal override RequestHandlerRootContext GetRoot() => this._Parent.GetRoot();

        public override IRequestHandlerContext CreateChild(ContextId id) {
            return new RequestHandlerContext(this, id);
        }

        public override IRequestHandlerFactory GetRequestHandlerFactory() {
            return this.GetRoot().GetRequestHandlerFactory();
        }

        public override TRequestHandler CreateRequestHandler<TRequestHandler>() {
            TRequestHandler result = this.GetRoot().GetRequestHandlerFactory().CreateRequestHandler<TRequestHandler>();
            return result;
        }
    }

    public class RequestHandlerRootContext : RequestHandlerContextBase, IRequestHandlerRootContext {
        private IRequestHandlerFactory? _RequestHandlerFactory;
        private Nullable<CancellationToken> _CancellationToken;

        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
        public RequestHandlerRootContext(IServiceProvider scopedServiceProvider) {
            this.ScopedServiceProvider = scopedServiceProvider;
        }
        public RequestHandlerRootContext(IServiceProvider scopedServiceProvider) {
            this.ScopedServiceProvider = scopedServiceProvider;
        }

        public IServiceProvider ScopedServiceProvider { get; }

        protected override ContextId GetId() => new ContextId(string.Empty, null);

        protected internal override RequestHandlerRootContext GetRoot() => this;

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
        public void SetGetCancellationToken(CancellationToken value) {
            this._CancellationToken = value;
        }
        public CancellationToken? GetCancellationToken() { return this._CancellationToken; }

        //public IResponseConverter GetResponseConverter() {
        //    throw new NotImplementedException();
        //}
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
