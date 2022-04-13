
using System.Collections.Generic;

namespace Brimborium.TypedStoredProcedure {
    public sealed record StoredProcedureResultSetNested(
        List<StoredProcedureResultSetNested> Children,
        bool IsSequential,
        StoredProcedureResultSet? ResultSet);
}
