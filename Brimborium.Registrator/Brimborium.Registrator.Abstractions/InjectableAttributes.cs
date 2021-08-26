using Microsoft.Extensions.DependencyInjection;

using System;

namespace Brimborium.Registrator {

    /// <summary>
    /// Annotate as injectable class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InjectableAttribute : Attribute {
        /// <summary>
        /// Create new instance by entry mode.
        /// </summary>
        /// <param name="mode"></param>
        public InjectableAttribute(ServiceLifetime mode) {
            Mode = mode;
        }

        /// <summary>
        /// Get injection mode
        /// </summary>
        public ServiceLifetime Mode { get; }

        /// <summary>
        /// Register class as itself or not.
        /// </summary>
        public bool AsSelf { get; init; }
    }

    /// <summary>
    /// Annotate as singleton class by singleton lifetime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonAttribute : InjectableAttribute {
        /// <summary>
        /// Create new instance.
        /// </summary>
        public SingletonAttribute() : base(ServiceLifetime.Singleton) {
        }
    }

    /// <summary>
    /// Annotate as scoped class by scoped lifetime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ScopedAttribute : InjectableAttribute {
        /// <summary>
        /// Create new instance.
        /// </summary>
        public ScopedAttribute() : base(ServiceLifetime.Scoped) {
        }
    }


    /// <summary>
    /// Annotate as transient class by transient lifetime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TransientAttribute : InjectableAttribute {
        /// <summary>
        /// Create new instance.
        /// </summary>
        public TransientAttribute() : base(ServiceLifetime.Transient) {
        }
    }

    /// <summary>
    /// Annotate as factory class by singleton lifetime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class FactoryAttribute : SingletonAttribute {
    }

    /// <summary>
    /// Annotate as repository class by scoped lifetime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RepositoryAttribute : ScopedAttribute {
    }

    /// <summary>
    /// Annotate as service class by scoped lifetime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ServiceAttribute : ScopedAttribute {
    }
}
