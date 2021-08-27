using Microsoft.Extensions.DependencyInjection;

using System;

namespace Brimborium.Registrator {

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ServiceDescriptorAttribute : Attribute {
        public ServiceDescriptorAttribute() : this(null) { }

        public ServiceDescriptorAttribute(Type? serviceType) : this(serviceType, ServiceLifetime.Transient) { }

        public ServiceDescriptorAttribute(Type? serviceType, ServiceLifetime lifetime) {
            ServiceType = serviceType;
            Lifetime = lifetime;
        }
        /// <summary>
        /// Register class as itself or not.
        /// </summary>
        public bool AsSelf { get; init; }

        /// <summary>
        /// Register this ServiceType
        /// </summary>
        public Type? ServiceType { get; init; }

        public ServiceLifetime Lifetime { get; init; }
    }

    /// <summary>
    /// Annotate as singleton class by singleton lifetime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class SingletonAttribute : ServiceDescriptorAttribute {
        public SingletonAttribute() : base(null, ServiceLifetime.Singleton) {
        }
        public SingletonAttribute(Type? serviceType) : base(serviceType, ServiceLifetime.Singleton) {
        }
    }

    /// <summary>
    /// Annotate as scoped class by scoped lifetime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ScopedAttribute : ServiceDescriptorAttribute {
        public ScopedAttribute() : base(null, ServiceLifetime.Scoped) {
        }
        public ScopedAttribute(Type? serviceType) : base(serviceType, ServiceLifetime.Scoped) {
        }
    }


    /// <summary>
    /// Annotate as transient class by transient lifetime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TransientAttribute : ServiceDescriptorAttribute {
        public TransientAttribute() : base(null, ServiceLifetime.Transient) {
        }
        public TransientAttribute(Type? serviceType) : base(serviceType, ServiceLifetime.Transient) {
        }
    }
}
