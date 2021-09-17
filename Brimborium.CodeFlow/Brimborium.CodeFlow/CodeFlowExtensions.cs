using Brimborium.Registrator;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;

namespace Brimborium.CodeFlow.RequestHandler {
    public static class CodeFlowExtensions {
        public static IServiceCollection AddRequestHandler(
            this IServiceCollection services,
            Func<ITypeSourceSelector, IImplementationTypeSelector>? fromAssemblies = null,
            bool alsoUsingAttributes = false,
            Action<ISelectorTarget>? actionRevisit = default
            ) {
            services.TryAddSingleton<IGlobalRequestHandlerFactory, GlobalRequestHandlerFactory>();
            services.TryAddScoped<IRequestHandlerFactory, ScopeRequestHandlerFactory>();
            services.TryAddScoped<IRequestHandlerSupport, RequestHandlerContextBuilder>();
            // services.TryAddScoped<IRequestHandlerRootContext, RequestHandlerRootContext>();
            // services.TryAddScoped<IRequestHandlerContext, RequestHandlerRootContext>();

            if (fromAssemblies is not null) {
                services.AddServicesWithRegistrator((a) => {
                    var assemblies = fromAssemblies(a);
                    AddRequestHandlerServices(assemblies);
                    if (alsoUsingAttributes) {
                        assemblies.AddClasses().UsingAttributes();
                    }
                },
                actionRevisit);
            }

            return services;
        }

        public static void AddRequestHandlerServices(IImplementationTypeSelector assemblies) {
            assemblies.AddClasses(c => {
                c.AssignableTo<ITypedRequestHandlerFactory>().WithoutAttribute<ServiceDescriptorAttribute>();
            }).AsImplementedInterfaces().WithSingletonLifetime();

            assemblies.AddClasses(c => {
                c.AssignableTo<IRequestHandler>().WithoutAttribute<ServiceDescriptorAttribute>();
            }).AsSelfWithInterfaces().WithTransientLifetime();

        }
    }
}
