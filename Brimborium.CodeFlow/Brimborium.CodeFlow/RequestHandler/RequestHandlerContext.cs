using Microsoft.Extensions.DependencyInjection;

using System;

namespace Brimborium.CodeFlow.RequestHandler {
    public abstract class RequestHandlerContextBase : IRequestHandlerContext {

        public RequestHandlerContextBase() {
        }

        protected internal abstract RequestHandlerRootContext GetRoot();
        protected internal abstract RequestHandlerContextBase GetParent();

        public abstract IRequestHandlerContext CreateChild();
    }

    public class RequestHandlerContext : RequestHandlerContextBase, IRequestHandlerContext {
        private readonly RequestHandlerContextBase _Parent;

        public RequestHandlerContext(RequestHandlerContextBase parent) {
            this._Parent = parent;
        }

        protected internal override RequestHandlerRootContext GetRoot() => this._Parent.GetRoot();

        protected internal override RequestHandlerContextBase GetParent() => this._Parent;

        public override IRequestHandlerContext CreateChild() {
            return new RequestHandlerContext(this);
        }

    }

    public class RequestHandlerRootContext : RequestHandlerContextBase, IRequestHandlerRootContext {
        private IGlobalRequestHandlerFactory? _RequestHandlerFactory;

        public RequestHandlerRootContext(IServiceProvider scopedServiceProvider) {
            this.ScopedServiceProvider = scopedServiceProvider;
        }

        public IServiceProvider ScopedServiceProvider { get; }

        protected internal override RequestHandlerRootContext GetRoot() => this;

        protected internal override RequestHandlerContextBase GetParent() => this;

        public override IRequestHandlerContext CreateChild() {
            return new RequestHandlerContext(this);
        }

        public IGlobalRequestHandlerFactory RequestHandlerFactory {
            get {
                return this.GetRequestHandlerFactory();
            }
            set {
                this._RequestHandlerFactory = value;
            }
        }

        public IGlobalRequestHandlerFactory GetRequestHandlerFactory() {
            return _RequestHandlerFactory ??= this.ScopedServiceProvider.GetRequiredService<IGlobalRequestHandlerFactory>();
        }
    }
}
