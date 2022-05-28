namespace Brimborium.CodeGeneration.SQLRelated {
    public class TypeMappingSqlCS {
        private Dictionary<string, string> _Mapping;

        public TypeMappingSqlCS() {
            this._Mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public TypeMappingSqlCS Add(string result, string? schema = default, string? table = default, string? column = default, SqlDataType? sqlDataType = default) {
            if ((schema is not null) && (table is not null) && (column is not null) && (sqlDataType is not null)) {
                this._Mapping.TryAdd($"{schema}.{table}.{column}.{sqlDataType}", result);
            }
            if ((schema is not null) && (table is not null) && (column is not null) && (sqlDataType is null)) {
                this._Mapping.TryAdd($"{schema}.{table}.{column}.", result);
            }
            if ((schema is null) && (table is null) && (column is not null) && (sqlDataType is not null)) {
                this._Mapping.TryAdd($"..{column}.{sqlDataType}", result);
            }
            if ((schema is null) && (table is null) && (column is not null) && (sqlDataType is null)) {
                this._Mapping.TryAdd($"..{column}.", result);
            }
            if ((schema is null) && (table is null) && (column is null) && (sqlDataType is not null)) {
                this._Mapping.TryAdd($"...{sqlDataType}", result);
            }
            return this;
        }

        public string ConvertTypeSqlToCS(ColumnInfo columnInfo) {
            return ConvertTypeSqlToCS(columnInfo.TableSchema, columnInfo.TableName, columnInfo.Name, columnInfo.SqlDataType);
        }

        public string ConvertTypeSqlToCS(string schema, string table, string column, SqlDataType sqlDataType) {
            string? result;
            if (this._Mapping.TryGetValue($"{schema}.{table}.{column}.{sqlDataType}", out result)) { return result; }
            if (this._Mapping.TryGetValue($"{schema}.{table}.{column}.", out result)) { return result; }
            if (this._Mapping.TryGetValue($"..{column}.{sqlDataType}", out result)) { return result; }
            if (this._Mapping.TryGetValue($"..{column}.", out result)) { return result; }
            if (this._Mapping.TryGetValue($"...{sqlDataType}", out result)) { return result; }
            return DefaultTypeMappingHelper.ConvertTypeSqlToCS(sqlDataType);
        }
    }
}