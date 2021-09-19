using Microsoft.Extensions.DependencyInjection;

namespace Brimborium.WebFlow.WebLogic {
    public static class WebFlowLogicExtensions { 
        public static IServiceCollection AddWebFlowLogicServices(
            this IServiceCollection services) {
            services.AddSingleton<IGnaRepository, GnaRepository>();
            return services;
        }
    }
}
