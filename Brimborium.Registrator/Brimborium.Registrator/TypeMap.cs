using System;
using System.Collections.Generic;

namespace Brimborium.Registrator
{
    internal struct TypeMap
    {
        public TypeMap(Type implementationType, IEnumerable<Type> serviceTypes)
        {
            this.ImplementationType = implementationType;
            this.ServiceTypes = serviceTypes;
        }

        public Type ImplementationType { get; }

        public IEnumerable<Type> ServiceTypes { get; }
    }
}
