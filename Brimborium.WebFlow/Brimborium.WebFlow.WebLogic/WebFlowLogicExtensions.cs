using Brimborium.CodeFlow.FlowSynchronization;
using Brimborium.CodeFlow.RequestHandler;
using Brimborium.WebFlow.Web.Communication;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Brimborium.WebFlow.WebLogic {
    public static class WebFlowLogicExtensions {
        public static IServiceCollection AddWebFlowLogicServices(
            this IServiceCollection services) {
            services.TryAddSingleton<IGnaRepository, GnaRepository>();
            services.TryAddSingleton<SyncTimer>((serviceProvider) => {
                var result = new SyncTimer(serviceProvider.GetRequiredService<ILogger<SyncTimer>>());
                serviceProvider.GetService<ISyncTimerHostedService>()?.Register(result);
                return result;
            });
            services.TryAddSingleton<SyncDictionary>((serviceProvider) => {
                var options = new SyncDictionaryOptions(
                    new SyncFactory(),
                    false,
                    System.TimeSpan.FromMinutes(30)
                    );
                return new SyncDictionary(
                    options,
                    serviceProvider.GetRequiredService<SyncTimer>());
            });
            

            services.TryAddScoped<SyncLockCollection, SyncLockCollection>();

            services.TryAddScoped<IRequestHandlerRootContext, RequestHandlerRootContext>();
            services.TryAddScoped<IGnaService, GnaService>();

            return services;
        }
    }
}
