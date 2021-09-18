using System;

using Brimborium.CodeFlow.RequestHandler;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Brimborium.WebFlow
{
    public static class WebFlowExtensions {
        public static IServiceCollection AddWebFlowServices(
            this IServiceCollection services) {
            services.AddSingleton<IRequestResultConverter, ConverterRequestHandlerResult>();
            return services;
        }
    }
}
