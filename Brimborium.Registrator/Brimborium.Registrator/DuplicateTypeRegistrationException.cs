using System;

namespace Brimborium.Registrator {
    public class DuplicateTypeRegistrationException : InvalidOperationException {
        public DuplicateTypeRegistrationException(Type serviceType)
            : base($"A service of type '{serviceType.ToFriendlyName()}' has already been registered.") {
            this.ServiceType = serviceType;
        }

        public Type ServiceType { get; }
    }
}
