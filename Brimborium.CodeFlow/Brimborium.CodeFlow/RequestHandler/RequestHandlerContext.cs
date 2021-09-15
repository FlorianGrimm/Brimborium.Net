using Microsoft.Extensions.DependencyInjection;

using System;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.RequestHandler {
    public abstract class RequestHandlerContextBase : IRequestHandlerContext {

        public RequestHandlerContextBase() {
        }
        public ContextId Id => this.GetId();

        protected abstract ContextId GetId();

        protected internal abstract RequestHandlerRootContext GetRoot();

        protected internal abstract RequestHandlerContextBase GetParent();

        public abstract IRequestHandlerContext CreateChild(ContextId id);

        public abstract IScopeRequestHandlerFactory GetRequestHandlerFactory();
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

        protected internal override RequestHandlerContextBase GetParent() => this._Parent;

        public override IRequestHandlerContext CreateChild(ContextId id) {
            return new RequestHandlerContext(this, id);
        }

        public override IScopeRequestHandlerFactory GetRequestHandlerFactory() {
            return this.GetRoot().GetRequestHandlerFactory();
        }
    }

    public class RequestHandlerRootContext : RequestHandlerContextBase, IRequestHandlerRootContext {
        private IScopeRequestHandlerFactory? _RequestHandlerFactory;

        public RequestHandlerRootContext(IServiceProvider scopedServiceProvider) {
            this.ScopedServiceProvider = scopedServiceProvider;
        }

        public IServiceProvider ScopedServiceProvider { get; }

        protected override ContextId GetId() => new ContextId(string.Empty, null);

        protected internal override RequestHandlerRootContext GetRoot() => this;

        protected internal override RequestHandlerContextBase GetParent() => this;

        public override IRequestHandlerContext CreateChild(ContextId id) {
            return new RequestHandlerContext(this, id);
        }

        public IScopeRequestHandlerFactory RequestHandlerFactory {
            get {
                return this.GetRequestHandlerFactory();
            }
            set {
                this._RequestHandlerFactory = value;
            }
        }

        public override IScopeRequestHandlerFactory GetRequestHandlerFactory() {
            return _RequestHandlerFactory ??= this.ScopedServiceProvider.GetRequiredService<IScopeRequestHandlerFactory>();
        }
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
