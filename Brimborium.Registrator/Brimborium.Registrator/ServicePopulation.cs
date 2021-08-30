using Microsoft.Extensions.DependencyInjection;

using System;

namespace Brimborium.Registrator {
    public record ServicePopulation(
        Type ImplementationType,
        Type[] ServiceTypes,
        bool IsIncludeSelf,
        ServiceLifetime? Lifetime,
        RegistrationStrategy? Strategy
        );
}
