
using Microsoft.SqlServer.Management.Smo;

using System.Linq;

namespace Brimborium.TypedStoredProcedure {
    public sealed record DatabaseStoredProcedure(
            StoredProcedure SP,
            StoredProcedureDefintion SPDefinition,
            MemberDefinition[] Parameters,
            StoredProcedureResultSet[] ResultSets
        ) {

        public string[] ResultSetsToStrings() {
            var result = this.ResultSets.Select((rs, idx) => {
                var columnNames = string.Join(", ", rs.Columns.Select(c => c.Name));
                return $"({idx} = {columnNames})";
            });
            return result.ToArray();
        }
    }
}
