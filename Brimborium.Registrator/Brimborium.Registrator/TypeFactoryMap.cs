using System;
using System.Collections.Generic;

namespace Brimborium.Registrator
{
    internal struct TypeFactoryMap
    {
        public TypeFactoryMap(Func<IServiceProvider, object> implementationFactory, IEnumerable<Type> serviceTypes)
        {
            this.ImplementationFactory = implementationFactory;
            this.ServiceTypes = serviceTypes;
        }

        public Func<IServiceProvider, object> ImplementationFactory { get; }

        public IEnumerable<Type> ServiceTypes { get; }
    }
}
