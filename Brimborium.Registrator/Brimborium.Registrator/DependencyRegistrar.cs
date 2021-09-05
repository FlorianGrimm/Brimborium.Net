using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Brimborium.Registrator {
    /// <summary>
    /// Scan and register all dependencies.
    /// </summary>
    public static class DependencyRegistrar {
        /// <summary>
        /// Scan app domain assemblies and register all dependencies that have any reguto attributes.
        /// </summary>
        /// <param name="services"></param>
        public static void AddAttributtedServices(this IServiceCollection services, Func<AssemblyName, bool>? predicate) {
            services.AddServicesWithRegistrator(scan => {
                var implementationTypeSelector = scan.FromApplicationDependencies(DependencyContext.Default,predicate);
                services.AddAttributtedServices(implementationTypeSelector);
            });
        }

        public static void AddAttributtedServices(this IServiceCollection services, params Assembly[] assemblies) {
            services.AddServicesWithRegistrator(scan => {
                var implementationTypeSelector = scan.FromAssemblies(assemblies);
                services.AddAttributtedServices(implementationTypeSelector);
            });
        }

        public static void AddAttributtedServices(this IServiceCollection services, IEnumerable<Assembly> assemblies) {
            services.AddServicesWithRegistrator(scan => {
                var implementationTypeSelector = scan.FromAssemblies(assemblies);
                services.AddAttributtedServices(implementationTypeSelector);
            });
        }

        /// <summary>
        /// Scan entry assemblies and register all dependencies that have any reguto attributes.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        public static void AddAttributtedServices(this IServiceCollection services, IImplementationTypeSelector implementationTypeSelector) {
            implementationTypeSelector.AddClasses().UsingAttributes();
        }
    }
}
