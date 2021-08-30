using System;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow
{
    public interface IContext { }

    public interface IRequestHandlerFactory<RH>
        where RH : IRequestHandler {
        RH CreateRequestHandler(IServiceProvider serviceProvider);
    }

    public interface IRequestHandlerFactoryChained<RH>: IRequestHandlerFactory<RH>
        where RH : IRequestHandler {
        //IRequestHandlerFactory<RH> Chain(IServiceProvider serviceProvider, IRequestHandlerFactory<RH> next);
    }

    public interface IRequestHandler { }
    public interface IRequestHandler<I, O> : IRequestHandler {
        public IRequestHandler<I, O> Prepare() { return this; }
        Task<O> ExecuteAsync(I parameters, IContext context);
    }

    public interface IRequestHandlerChained :IRequestHandler { }
    public interface IRequestHandlerChained<I, O> : IRequestHandlerChained, IRequestHandler<I, O> {
        IRequestHandler<I, O> Chain(IRequestHandler<I, O> next);
        //IRequestHandler<I, O> Prepare();
        //Task<O> ExecuteAsync(I parameters, IContext context);
    }
}
