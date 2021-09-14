using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Brimborium.CodeFlow.RequestHandler;

namespace Microsoft.Extensions.DependencyInjection {
    public static class ServicesExtension {
        public static IServiceCollection AddRequestHandler(this IServiceCollection services) {
            services.AddSingleton<IGlobalRequestHandlerFactory, GlobalRequestHandlerFactory>();
            services.AddScoped<IRequestHandlerRootContext, RequestHandlerRootContext>();
            services.AddScoped<IRequestHandlerContext, RequestHandlerRootContext>();
            return services;
        }
    }
}
