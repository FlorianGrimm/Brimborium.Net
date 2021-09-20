using Microsoft.Extensions.DependencyInjection;

namespace Brimborium.CodeFlow.RequestHandler {
    public static class RequestHandlerContextExtensions {
        public static System.IServiceProvider GetScopedServiceProvider(this IRequestHandlerContext context) {
            IRequestHandlerRootContextInternal requestHandlerRootContextInternal
                = (context as IRequestHandlerRootContextInternal)
                ?? ((IRequestHandlerContextInternal)context).GetRequestHandlerRootContext();
            
            return requestHandlerRootContextInternal.ScopedServiceProvider;
        }

        public static System.Security.Claims.ClaimsPrincipal GetUser(this IRequestHandlerContext context) {
            IRequestHandlerRootContextInternal requestHandlerRootContextInternal
                = (context as IRequestHandlerRootContextInternal)
                ?? ((IRequestHandlerContextInternal)context).GetRequestHandlerRootContext();
            return requestHandlerRootContextInternal.GetUser() ?? new System.Security.Claims.ClaimsPrincipal();
        }

        public static T GetRequiredService<T>(this IRequestHandlerContext context)
            where T : notnull {
            IRequestHandlerRootContextInternal requestHandlerRootContextInternal
                = (context as IRequestHandlerRootContextInternal)
                ?? ((IRequestHandlerContextInternal)context).GetRequestHandlerRootContext();
            var scopedServiceProvider
                = ((context as IRequestHandlerRootContextInternal)?.ScopedServiceProvider)
                ?? ((IRequestHandlerContextInternal)context).GetRequestHandlerRootContext().ScopedServiceProvider;
            return requestHandlerRootContextInternal.ScopedServiceProvider.GetRequiredService<T>();
        }
    }
}
