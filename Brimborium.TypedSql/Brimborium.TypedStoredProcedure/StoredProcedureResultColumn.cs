
using System.Text;

namespace Brimborium.TypedStoredProcedure {
    public record StoredProcedureResultColumn(
        int Index,
        string Name,
        string TypeName,
        int TypeSize,
        bool IsNullable) {

        public int IsStringType() => SQLUtility.IsStringType(this.TypeName, this.TypeSize);

        protected virtual bool PrintMembers(StringBuilder builder) {
            builder.Append(this.Name);
            builder.Append(":");
            builder.Append(this.TypeName);
            if (this.TypeSize > 0) {
                builder.Append("(");
                builder.Append(this.TypeSize);
                builder.Append(")");
            }
            if (this.IsNullable) {
                builder.Append(" NULLable");
            } else {
                builder.Append(" NOT NULL");
            }
            return true;
        }

        public override string ToString() {
            var builder = new StringBuilder();
            this.PrintMembers(builder);
            return builder.ToString();
        }

        internal static bool IsStructuralEqual(StoredProcedureResultColumn storedProcedureResultColumn1, StoredProcedureResultColumn storedProcedureResultColumn2) {

            return false;
        }
    }
}
