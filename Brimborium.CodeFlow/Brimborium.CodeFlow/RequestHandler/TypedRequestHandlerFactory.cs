using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace Brimborium.CodeFlow.RequestHandler {
    //public class TypedRequestHandlerFactory<TRequest, TResponse, TIRequestHandler>
    //     : ITypedRequestHandlerFactory<TIRequestHandler>
    //     where TIRequestHandler : notnull, IRequestHandler<TRequest, TResponse> {
    //    private readonly IServiceProvider _GlobalServiceProvider;

    //    public TypedRequestHandlerFactory(IServiceProvider globalServiceProvider) {
    //        this._GlobalServiceProvider = globalServiceProvider;
    //    }
    //    public TIRequestHandler CreateTypedRequestHandler(IServiceProvider scopedServiceProvider) {
    //        return this._GlobalServiceProvider.GetRequiredService<TIRequestHandler>();
    //    }
    //}
    /*
     * public interface IRequestHandler<TRequest, TResponse> : IRequestHandler {
        Task<TResponse> ExecuteAsync(TRequest request, IRequestHandlerContext context, CancellationToken cancellationToken = default);
    }
     public interface IRequestHandlerFactory {
        TRequestHandler CreateRequestHandler<TRequestHandler>()
            where TRequestHandler : notnull, IRequestHandler;
    }
     */
}
