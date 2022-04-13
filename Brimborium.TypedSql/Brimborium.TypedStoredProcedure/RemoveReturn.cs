
using System.Collections.Generic;

namespace Brimborium.TypedStoredProcedure {
    internal class RemoveReturn : Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCodeObjectVisitor {
        public RemoveReturn() {
            this.Locations = new List<RemoveReturnSpan>();
        }

        public List<RemoveReturnSpan> Locations { get; }

        public override void Visit(Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlReturnStatement codeObject) {
            //codeObject.StartLocation.Offset
            //codeObject.EndLocation.Offset
            this.Locations.Add(new RemoveReturnSpan(codeObject.StartLocation.Offset, codeObject.EndLocation.Offset));
        }
    }
}
