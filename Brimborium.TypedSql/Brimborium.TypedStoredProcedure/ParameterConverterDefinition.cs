
using System;

namespace Brimborium.TypedStoredProcedure {
    public sealed record ParameterConverterDefinition(Type CsType) {
        public static ParameterConverterDefinition TypeOf<T>()
            => new ParameterConverterDefinition(typeof(T));
    }
}
