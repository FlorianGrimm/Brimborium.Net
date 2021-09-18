using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using System.Security.Claims;
using System.Threading;

namespace Brimborium.CodeFlow.RequestHandler {
    public static class ControllerBaseExtensions {

        public static GetRequestHandlerRootContextResult GetRequestHandlerRootContext(
            this IRequestHandlerSupport requestHandlerSupport,
            ControllerBase controllerBase,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
            ) {
            var requestServices = requestHandlerSupport.GetScopeServiceProvider();
            //
            {
                if (requestHandlerSupport.TryGetRequestHandlerRootContext(out var context)) {
                    var rootContext = context as IRequestHandlerRootContextInternal;
                    CancellationToken cancellationToken = rootContext?.GetCancellationToken() ?? CancellationToken.None;
                    var user = (rootContext?.GetUser()) ?? (new ClaimsPrincipal());
                    var responseConverter = requestServices.GetRequiredService<IRequestResultConverter>();
                    return new GetRequestHandlerRootContextResult(context, user, cancellationToken, responseConverter);
                }
            }
            {
                //static ActivitySource s_source = new ActivitySource("Sample.DistributedTracing");
                //var a = System.Diagnostics.Activity.Current;
#warning TODO https://docs.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-collection-walkthroughs

                //var name = $"{controllerBase.GetType().FullName}.{memberName}";
                //Activity? activity = s_source.StartActivity(name, ActivityKind.Server)!;
                //activity.Dispose(); 
                //var rootContext = new RequestHandlerRootContext(requestServices);
                var rootContext = (IRequestHandlerRootContextInternal)requestServices.GetRequiredService<IRequestHandlerRootContext>();
                Microsoft.AspNetCore.Http.HttpContext? httpContext = controllerBase.HttpContext;
                CancellationToken cancellationToken = (httpContext?.RequestAborted) ?? (CancellationToken.None);
                ClaimsPrincipal user = (httpContext?.User) ?? (new ClaimsPrincipal());
                rootContext.SetCancellationToken(cancellationToken);
                var responseConverter = requestServices.GetRequiredService<IRequestResultConverter>();
                requestHandlerSupport.SetRequestHandlerContext(rootContext);
                return new GetRequestHandlerRootContextResult(rootContext, user, cancellationToken, responseConverter);
            }
        }
    }

    public struct GetRequestHandlerRootContextResult {
        public readonly IRequestHandlerRootContext Context;
        public readonly ClaimsPrincipal User;
        public readonly CancellationToken CancellationToken;
        public readonly IRequestResultConverter ResponseConverter;

        public GetRequestHandlerRootContextResult(
            IRequestHandlerRootContext context,
            ClaimsPrincipal user,
            CancellationToken cancellationToken,
            IRequestResultConverter responseConverter) {
            this.Context = context;
            this.User = user;
            this.CancellationToken = cancellationToken;
            this.ResponseConverter = responseConverter;
        }

        public void Deconstruct(
            out IRequestHandlerRootContext context,
            out ClaimsPrincipal user,
            out CancellationToken cancellationToken,
            out IRequestResultConverter responseConverter) {
            context = this.Context;
            user = this.User;
            cancellationToken = this.CancellationToken;
            responseConverter = this.ResponseConverter;
        }
    }
}
