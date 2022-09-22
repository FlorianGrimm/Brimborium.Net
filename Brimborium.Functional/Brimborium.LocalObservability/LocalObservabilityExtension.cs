using Brimborium.LocalObservability;

namespace Microsoft.Extensions.DependencyInjection {
    public static class LocalObservabilityExtension {
        public static IServiceCollection AddIgnoreLocalObservabilitySink(
            this IServiceCollection services
            ) {
            services.AddSingleton<ILocalObservabilitySink, IgnoreLocalObservabilitySink>();
            return services;
        }

        public static IServiceCollection AddSimpleLocalObservabilitySink(
            this IServiceCollection services
            ) {
            services.AddSingleton<SimpleLocalObservabilitySink, SimpleLocalObservabilitySink>();
            services.AddSingleton<ILocalObservabilitySink>(
                    (serviceProvider) => serviceProvider.GetRequiredService<SimpleLocalObservabilitySink>()
                );
            services.AddSingleton<ISimpleLocalObservabilityCollector>(
                    (serviceProvider) => serviceProvider.GetRequiredService<SimpleLocalObservabilitySink>()
                );
            return services;
        }
    }
}
