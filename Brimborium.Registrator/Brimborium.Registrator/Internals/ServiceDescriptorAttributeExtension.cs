using System;
using System.Collections.Generic;

namespace Brimborium.Registrator.Internals {
    public static class ServiceDescriptorAttributeExtension {
        public static IEnumerable<Type> GetServiceTypes(this ServiceDescriptorAttribute that, Type fallbackType) {
            if (that.ServiceType is null) {
                yield return fallbackType;

                var fallbackTypes = fallbackType.GetBaseTypes();

                foreach (var type in fallbackTypes) {
                    if (type == typeof(object)) {
                        continue;
                    }

                    yield return type;
                }

                yield break;
            } else {
                if (!fallbackType.IsAssignableTo(that.ServiceType)) {
                    throw new InvalidOperationException($@"Type ""{fallbackType.ToFriendlyName()}"" is not assignable to ""{that.ServiceType.ToFriendlyName()}"".");
                }

                yield return that.ServiceType;
            }
        }
    }
}
