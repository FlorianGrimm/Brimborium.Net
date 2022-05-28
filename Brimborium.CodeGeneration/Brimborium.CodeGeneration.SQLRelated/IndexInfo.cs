
using SmoIndex = Microsoft.SqlServer.Management.Smo.Index;

namespace Brimborium.CodeGeneration.SQLRelated {
    public sealed record IndexInfo(
        SmoIndex Index,
        string Schema,
        string Name,
        List<ColumnInfo> Columns
        ) {
        public static IndexInfo Create(SmoIndex index) {
            var table = (Table)index.Parent;
            return new IndexInfo(
                index,
                table.Schema,
                index.Name,
                new List<ColumnInfo>());
        }
    }
}
