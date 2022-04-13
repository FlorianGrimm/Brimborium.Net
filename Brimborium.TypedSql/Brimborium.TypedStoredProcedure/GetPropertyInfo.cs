
using System.Collections.Generic;

namespace Brimborium.TypedStoredProcedure {
    public sealed record GetPropertyInfo(
            Dictionary<string, System.Reflection.ParameterInfo> ctorParameters,
            Dictionary<string, System.Reflection.PropertyInfo> readableProperties,
            Dictionary<string, System.Reflection.PropertyInfo> writableProperties,
            Dictionary<string, CalculateMappingProperty> allProperties
        );
}
