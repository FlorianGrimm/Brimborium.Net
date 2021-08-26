using System;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Brimborium.Registrator {
    /// <summary>
    /// Scan and register all dependencies.
    /// </summary>
    public static class DependencyRegistrar {
        /// <summary>
        /// Scan app domain assemblies and register all dependencies that have any reguto attributes.
        /// </summary>
        /// <param name="services"></param>
        public static void AddReguto(this IServiceCollection services) {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            services.AddReguto(assemblies);
        }

        /// <summary>
        /// Scan entry assemblies and register all dependencies that have any reguto attributes.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        public static void AddReguto(this IServiceCollection services, params Assembly[] assemblies) {
            services.ScanAndRegister(assemblies, ServiceLifetime.Singleton, true);
            services.ScanAndRegister(assemblies, ServiceLifetime.Scoped, true);
            services.ScanAndRegister(assemblies, ServiceLifetime.Transient, true);
            services.ScanAndRegister(assemblies, ServiceLifetime.Singleton, false);
            services.ScanAndRegister(assemblies, ServiceLifetime.Scoped, false);
            services.ScanAndRegister(assemblies, ServiceLifetime.Transient, false);
        }

        private static void ScanAndRegister(this IServiceCollection services,
                                            Assembly[] assemblies,
                                            ServiceLifetime injectionMode,
                                            bool asSelf) {
            services.Scan(scan => {
                var serviceTypeSelector = scan.FromAssemblies(assemblies)
                                              .AddClasses(classes => classes.Where(type => {
                                                  var injectionAttributeType = typeof(InjectableAttribute);
                                                  var customAttribute = type.GetCustomAttributes()
                                                                             .FirstOrDefault(attribute => {
                                                                                 var attributeType = attribute.GetType();

                                                                                 return attributeType == injectionAttributeType ||
                                                                                        attributeType.IsSubclassOf(injectionAttributeType);
                                                                             });

                                                  if (customAttribute is null) {
                                                      return false;
                                                  }

                                                  var injectableAttribute = (InjectableAttribute)customAttribute;

                                                  return injectableAttribute.AsSelf == asSelf &&
                                                         injectableAttribute.Mode == injectionMode;
                                              }));

                var lifeTimeSelector = asSelf ? serviceTypeSelector.AsSelf() : serviceTypeSelector.AsImplementedInterfaces();
                switch (injectionMode) {
                    case ServiceLifetime.Singleton: {
                            lifeTimeSelector.WithSingletonLifetime();
                            break;
                        }
                    case ServiceLifetime.Scoped: {
                            lifeTimeSelector.WithScopedLifetime();
                            break;
                        }
                    case ServiceLifetime.Transient: {
                            lifeTimeSelector.WithTransientLifetime();
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(injectionMode), injectionMode, null);
                }
            });
        }
    }
}
