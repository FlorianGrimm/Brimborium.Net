
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Brimborium.TypedStoredProcedure {
    public sealed record StoredProcedureResultSet(int Index, StoredProcedureResultColumn[] Columns) {
        public static bool IsStructuralEqual(StoredProcedureResultSet a, StoredProcedureResultSet b) {
            if (a.Columns.Length == b.Columns.Length) {
                for (var idx = 0; idx < a.Columns.Length; idx++) {
                    if (StoredProcedureResultColumn.IsStructuralEqual(a.Columns[idx], b.Columns[idx])) {
                        //OK
                    } else {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        public override string ToString() {
            var builder = new StringBuilder();
            this.PrintMembers(builder);
            return builder.ToString();
        }

        private bool PrintMembers(StringBuilder builder) {
            builder.Append(this.Index).Append(" : (");
            if (this.Columns is object) {
                foreach (var c in this.Columns) {
                    builder.Append(c);
                    builder.Append(", ");
                }
            }
            builder.Append(")");
            return true;
        }

        internal bool TryMergeResultSet(
            StoredProcedureResultSet other,
            Action<string> errorWriteLine,
            [MaybeNullWhen(false)] out StoredProcedureResultSet resultSet) {
            if (this.Columns.Length != other.Columns.Length) {
                errorWriteLine($"#error Column Count are different: {this.Columns.Length} != {other.Columns.Length}");
                goto fail;
            }
            var result = new List<StoredProcedureResultColumn>();
            for (var idx = 0; idx < this.Columns.Length; idx++) {
                var thisColumn = this.Columns[idx];
                var otherColumn = other.Columns[idx];
                if (!string.Equals(thisColumn.Name, otherColumn.Name, StringComparison.OrdinalIgnoreCase)) {
                    errorWriteLine($"#error ColumnName are different: {idx} - ColumnName:{thisColumn.Name} != {otherColumn.Name}");
                    goto fail;
                }

                var thisStringType = thisColumn.IsStringType();
                var otherStringType = otherColumn.IsStringType();
                var thisColumnTypeName = thisColumn.TypeName;
                if (thisColumn.TypeName != otherColumn.TypeName) {
                    if (thisStringType == 0 && thisStringType == 0) {
                        errorWriteLine($"#error ColumnType are different: {idx} {thisColumn.Name} - ColumnType:{thisColumn.TypeName} != {otherColumn.TypeName}");
                        goto fail;
                    }
                } else if (thisStringType < otherStringType) {
                    thisColumnTypeName = SQLUtility.GetStringType(otherColumn.TypeName, otherStringType);
                }
                result.Add(
                    new StoredProcedureResultColumn(
                        thisColumn.Index,
                        thisColumn.Name,
                        thisColumnTypeName,
                        MaxSize(thisColumn.TypeSize, otherColumn.TypeSize), thisColumn.IsNullable || otherColumn.IsNullable)
                    );
            }
            resultSet = this;
            return true;

        fail:
            resultSet = null;
            return false;
        }

        private static int MaxSize(int a, int b) {
            if (a == -1 || b == -1) { return -1; }
            return Math.Max(a, b);
        }
    }
}
