namespace Brimborium.CodeGeneration.SQLRelated {
    public sealed record ColumnInfo(
        Column Column,
        string Name,
        string TableName,
        string TableSchema,
        DataTypeInfo DataTypeInfo,
        bool Nullable
        ) {
        public static ColumnInfo Create(
            Column column
        ) {
            var table = (Table)column.Parent;
            return new ColumnInfo(
                column,
                column.Name,
                table.Name,
                table.Schema,
                DataTypeInfo.Create(column.DataType),
                column.Nullable
                ) {
                PrimaryKeyIndexPosition = -1,
                ClusteredIndexPosition = -1
            };
        }
        public SqlDataType SqlDataType => this.DataTypeInfo.SqlDataType;
        
        public int PrimaryKeyIndexPosition { get; set; }
        public int ClusteredIndexPosition { get; set; }
        public Hashtable ExtraInfo { get; } = new Hashtable();
        public string? ParameterSqlDataType { get; set; }

        public Type? PropertyType { get; set; }
        public string? PropertyTypeName { get; set; }


        public string GetPropertyType(TypeMappingSqlCS typeMappingSqlCS) {
            if (this.PropertyTypeName is not null) {
                return this.PropertyTypeName;
            }
            if (this.PropertyType is not null) {
                var propertyTypeName = this.PropertyType.FullName ?? throw new InvalidOperationException("PropertyType");
                this.PropertyTypeName = propertyTypeName;
                return propertyTypeName;
            }
            {
                var table = (Table)Column.Parent;
                var csType = typeMappingSqlCS.ConvertTypeSqlToCS(
                    TableSchema,
                    TableName,
                    Name,
                    SqlDataType);
                this.PropertyTypeName = csType;
                return csType;
            }
        }

        // alias
        public Func<string?, string>? ReadExpression { get; set; }
        public Func<string?, string?, string>? ReadAsExpression { get; set; }
        public Func<string?, string?, string>? ReadNamedExpression { get; set; }


        public bool Identity => this.Column.Identity;

        public string GetNameQ(string? sourceAlias = null) => string.IsNullOrEmpty(sourceAlias) ? $"[{this.Name}]" : $"{sourceAlias}.[{this.Name}]";

        public string GetNamePrefixed(string prefix) => prefix + this.Name;

        public string GetReadQ(string? sourceAlias = null) {
            if (sourceAlias is not null && sourceAlias.Length == 0) { sourceAlias = null; }

            if (this.ReadExpression is not null) {
                return this.ReadExpression(sourceAlias);
            } else {
                return this.GetNameQ(sourceAlias);
            }
        }

        public string GetReadAsQ(string? sourceAlias = null, string? expressionAlias = null) {
            if (sourceAlias is not null && sourceAlias.Length == 0) { sourceAlias = null; }
            if (expressionAlias is not null && expressionAlias.Length == 0) { expressionAlias = null; }

            if (this.ReadAsExpression is not null) {
                return this.ReadAsExpression(sourceAlias, expressionAlias);
            } else {
                var n = this.GetReadQ(sourceAlias);
                var a = expressionAlias ?? this.GetNameQ();
                return $"{n} as {a}";
            }
        }

        public string GetReadNamedQ(string? sourceAlias = null, string? expressionAlias = null) {
            if (sourceAlias is not null && sourceAlias.Length == 0) { sourceAlias = null; }
            if (expressionAlias is not null && expressionAlias.Length == 0) { expressionAlias = null; }

            if (this.ReadNamedExpression is not null) {
                return this.ReadNamedExpression(sourceAlias, expressionAlias);
            } else {
                var n = this.GetReadQ(sourceAlias);
                var a = expressionAlias ?? this.GetNameQ();
                return $"{a} = {n}";
            }
        }

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
        public T GetExtraInfo<T>(string name, T defaultValue) {
            var result = this.ExtraInfo[name];
            if (result is T resultT) {
                return resultT;
            } else {
                return defaultValue;
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
