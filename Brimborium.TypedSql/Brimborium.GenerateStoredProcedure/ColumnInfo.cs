using System.Collections;

using Microsoft.SqlServer.Management.Smo;

namespace Brimborium.GenerateStoredProcedure {
    public sealed record ColumnInfo(
        Column Column,
        string Name
        ) {

        public int PrimaryKeyIndexPosition { get; set; }
        public int ClusteredIndexPosition { get; set; }
        public Hashtable ExtraInfo { get; } = new Hashtable();
        public string? ParameterSqlDataType { get; set; }

        public static ColumnInfo Create(
            Column column
            ) {
            return new ColumnInfo(
                column,
                column.Name) {
                PrimaryKeyIndexPosition = -1,
                ClusteredIndexPosition = -1
            };
        }
        public bool Identity => this.Column.Identity;
        public string GetNameQ(string? alias = null) => string.IsNullOrEmpty(alias) ? $"[{this.Name}]" : $"{alias}.[{this.Name}]";
        public string GetNamePrefixed(string prefix) => prefix + this.Name;
        public string GetSqlDataType(bool addNotNull = false) {
            var dataType = this.Column.DataType;
            var sqlName = dataType.GetSqlName(dataType.SqlDataType);
            if (dataType.IsStringType) {
                if (dataType.MaximumLength < 0) {
                    sqlName = $"{sqlName}(MAX)";
                } else {
                    sqlName = $"{sqlName}({dataType.MaximumLength})";
                }
            } else if (dataType.SqlDataType == SqlDataType.UserDefinedDataType) {
                sqlName = string.Empty;
            } else if (dataType.SqlDataType == SqlDataType.UserDefinedTableType) {
                sqlName = string.Empty;
            } else if (dataType.SqlDataType == SqlDataType.UserDefinedType) {
                sqlName = string.Empty;
            }
            if (addNotNull) {
                return sqlName + this.GetNotNull();
            } else {
                return sqlName;
            }
        }
        public string GetParameterSqlDataType(bool addNotNull = false) {
            if (string.IsNullOrEmpty(this.ParameterSqlDataType)) {
                return this.GetSqlDataType(addNotNull);
            } else {
                if (addNotNull) {
                    return this.ParameterSqlDataType + this.GetNotNull();
                } else {
                    return this.ParameterSqlDataType;
                }
            }
        }

        public string GetNotNull() {
            if (this.Column.Nullable) {
                return " NULL";
            } else {
                return " NOT NULL";
            }
        }
    }
}
