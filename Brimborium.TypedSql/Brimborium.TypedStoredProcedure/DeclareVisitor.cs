
using System.Collections.Generic;

namespace Brimborium.TypedStoredProcedure {
    internal class DeclareVisitor : Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCodeObjectRecursiveVisitor {
        public DeclareVisitor() {
            this.Result = new List<Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlDeclareStatement>();
        }

        public List<Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlDeclareStatement> Result { get; }

        public override void Visit(Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlInlineTableVariableDeclareStatement statement) {
            this.Result.Add(statement);
        }
        public override void Visit(Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlVariableDeclareStatement statement) {
            this.Result.Add(statement);
        }
    }
}
