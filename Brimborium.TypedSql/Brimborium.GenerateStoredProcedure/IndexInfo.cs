
using Microsoft.SqlServer.Management.Smo;

using System.Collections.Generic;

namespace Brimborium.GenerateStoredProcedure {
    public sealed record IndexInfo(
        Index Index,
        string Schema,
        string Name,
        List<ColumnInfo> Columns
        ) {
        public static IndexInfo Create(Index index) {
            var table = (Table)index.Parent;
            return new IndexInfo(
                index,
                table.Schema,
                index.Name,
                new List<ColumnInfo>());
        }
    }
}
