
using System;

namespace Brimborium.TypedStoredProcedure {
    public sealed record ReaderMethodDefinition(
        Microsoft.SqlServer.Management.Smo.SqlDataType SqlDataType,
        string ReaderMethod,
        Type ReadReturnType,
        string ReaderMethodQ,
        Type ReadReturnTypeQ
        );
}
