
using System;

namespace Brimborium.TypedStoredProcedure {
    public sealed record TypeConverterDefinition(
        Type FromType,
        Type ToType,
        string Converter);
}
