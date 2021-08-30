using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;

namespace Brimborium.Registrator {
    public record ServicePopulation(
        Type ImplementationType,
        Type[] ServiceTypes,
        bool IsIncludeSelf,
        ServiceLifetime? Lifetime,
        RegistrationStrategy? Strategy
        ) {
        public static ServicePopulation Create(
            Type ImplementationType,
            Type[] ServiceTypes,
            bool IsIncludeSelf,
            ServiceLifetime? Lifetime,
            RegistrationStrategy? Strategy
            ) {
            if (ServiceTypes.Length == 1 && ImplementationType.Equals(ServiceTypes[0])) {
                return new ServicePopulation(
                    ImplementationType,
                    new Type[0],
                    true,
                    Lifetime,
                    Strategy);
            } else if (ServiceTypes.Length > 0 && ServiceTypes.Contains(ImplementationType)) {
                return new ServicePopulation(
                    ImplementationType,
                    ServiceTypes.Where(t=>!ImplementationType.Equals(t)).ToArray(),
                    true,
                    Lifetime,
                    Strategy);
            }
            return new ServicePopulation(
                ImplementationType,
                ServiceTypes,
                IsIncludeSelf,
                Lifetime,
                Strategy);
        }
    };
}
