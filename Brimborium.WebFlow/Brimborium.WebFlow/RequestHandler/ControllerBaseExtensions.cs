using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.CodeFlow.RequestHandler;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

// namespace Brimborium.WebFlow.RequestHandler

namespace Brimborium.CodeFlow.RequestHandler {
    public static class ControllerBaseExtensions {
        //static ActivitySource s_source = new ActivitySource("Sample.DistributedTracing");

        public static GetRequestHandlerRootContextResult GetRequestHandlerRootContext(
            this IRequestHandlerSupport requestHandlerSupport,
            ControllerBase controllerBase,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
            ) {
            if (requestHandlerSupport.TryGetRequestHandlerRootContext(out var context)) {
                CancellationToken cancellationToken = (context as RequestHandlerRootContext)?.GetCancellationToken() ?? CancellationToken.None;
                //(context as RequestHandlerRootContext) ?.GetResponseConverter()
                var requestServices = requestHandlerSupport.GetScopeServiceProvider();
                var responseConverter = requestServices.GetRequiredService<IRequestHandlerResultConverter>();
                return new GetRequestHandlerRootContextResult(context, cancellationToken, responseConverter);
            } else {
                var requestServices = requestHandlerSupport.GetScopeServiceProvider();

                //var a = System.Diagnostics.Activity.Current;
#warning TODO https://docs.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-collection-walkthroughs

                //var name = $"{controllerBase.GetType().FullName}.{memberName}";
                //Activity? activity = s_source.StartActivity(name, ActivityKind.Server)!;
                //activity.Dispose(); 
                CancellationToken cancellationToken = (controllerBase.HttpContext?.RequestAborted) ?? CancellationToken.None;
                var responseConverter = requestServices.GetRequiredService<IRequestHandlerResultConverter>();
                context = new RequestHandlerRootContext(requestServices, cancellationToken);
                requestHandlerSupport.SetRequestHandlerContext(context);
                return new GetRequestHandlerRootContextResult(context, cancellationToken, responseConverter);
            }
        }

        //public static ActionResult<TResult> ConvertToActionResult<TResponse, TResult>(
        //    this ControllerBase controllerBase,
        //    RequestHandlerResult<TResponse> responseResult,
        //    Func<TResponse, TResult> extractValue
        //    ) {
        //    if (responseResult.TryGetValue(out var value)) {
        //        return extractValue(value);
        //    }
        //    if (responseResult.TryGetResult(out var result)) {
        //        if (result is RequestHandlerResulttOK requestResultOK) {
        //            if (requestResultOK.Value is TResponse response) {
        //                return extractValue(response);
        //            }
        //        }
        //        var responseConverter = controllerBase.HttpContext.RequestServices.GetRequiredService<IRequestHandlerResultConverter>();
        //        return responseConverter.Convert<TResult>(controllerBase, responseResult);
        //    }
        //    return controllerBase.BadRequest();
        //}
    }
    public struct GetRequestHandlerRootContextResult {
        public readonly IRequestHandlerRootContext Context;
        public readonly CancellationToken CancellationToken;
        public readonly IRequestHandlerResultConverter ResponseConverter;

        public GetRequestHandlerRootContextResult(IRequestHandlerRootContext context, CancellationToken cancellationToken, IRequestHandlerResultConverter responseConverter) {
            this.Context = context;
            this.CancellationToken = cancellationToken;
            this.ResponseConverter = responseConverter;
        }

        public void Deconstruct(out IRequestHandlerRootContext context, out CancellationToken cancellationToken, out IRequestHandlerResultConverter responseConverter) {
            context = this.Context;
            cancellationToken = this.CancellationToken;
            responseConverter = this.ResponseConverter;
        }
    }

}
