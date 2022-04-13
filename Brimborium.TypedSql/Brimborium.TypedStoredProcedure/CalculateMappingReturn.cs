
using System.Collections.Generic;

namespace Brimborium.TypedStoredProcedure {
    public sealed record CalculateMappingReturn(
        Dictionary<string, CalculateMappingProperty> AllProperties,
        CalculateMappingReturnItem[] AllMappings,
        CalculateMappingReturnItem[] CtorParameter,
        CalculateMappingReturnItem[] WriteProperties,
        CalculateMappingReturnItem[] ReadListProperties);
}
