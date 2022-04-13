
using System.Collections.Generic;
using System.Linq;

namespace Brimborium.GenerateStoredProcedure {
    public sealed record DatabaseInfo() {
        public List<TableInfo> Tables { get; init; } = new List<TableInfo>();
        public List<ForeignKeyInfo> ForeignKey
            => this.Tables.SelectMany(t => t.ForeignKeys).ToList();
    };
}
