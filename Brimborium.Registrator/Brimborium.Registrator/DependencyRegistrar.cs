using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Brimborium.Registrator {
    /// <summary>
    /// Scan and register all dependencies.
    /// </summary>
    public static class DependencyRegistrar {
        public static bool FilterAssembly(AssemblyName name) {
            return !(name.Name ?? string.Empty).StartsWith("System.");
        }

        /// <summary>
        /// Scan app domain assemblies and register all dependencies that have any reguto attributes.
        /// </summary>
        /// <param name="services"></param>
        public static void AddAttributtedServices(this IServiceCollection services, Func<AssemblyName, bool>? predicate) {
            services.Scan(scan => {
                var implementationTypeSelector = scan.FromApplicationDependencies(predicate ?? FilterAssembly);
                services.AddAttributtedServices(implementationTypeSelector);
            });
        }

        public static void AddAttributtedServices(this IServiceCollection services, params Assembly[] assemblies) {
            services.Scan(scan => {
                var implementationTypeSelector = scan.FromAssemblies(assemblies);
                services.AddAttributtedServices(implementationTypeSelector);
            });
        }

        public static void AddAttributtedServices(this IServiceCollection services, IEnumerable<Assembly> assemblies) {
            services.Scan(scan => {
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
