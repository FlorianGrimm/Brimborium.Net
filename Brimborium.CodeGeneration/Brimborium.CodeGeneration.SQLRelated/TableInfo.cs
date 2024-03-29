﻿namespace Brimborium.CodeGeneration.SQLRelated {
    public sealed record TableInfo(
            Table Table,
            string Schema,
            string Name,
            List<ColumnInfo> Columns,
            ColumnInfo? ColumnRowversion,
            IndexInfo IndexPrimaryKey,
            IndexInfo? IndexClustered,
            bool ClusteredIndexContainsPrimaryKey,
            List<ForeignKeyInfo> ForeignKeys,
            List<ForeignKeyInfo> ForeignKeysReferenced,
            List<IndexInfo> Indices
        ) {
        public static TableInfo Create(
            Table table,
            List<ColumnInfo> Columns,
            ColumnInfo? ColumnRowversion,
            IndexInfo IndexPrimaryKey,
            IndexInfo? IndexClustered,
            bool clusteredIndexContainsPrimaryKey,
            List<IndexInfo> Indices
            ) {
            return new TableInfo(
                    table,
                    table.Schema,
                    table.Name,
                    Columns,
                    ColumnRowversion,
                    IndexPrimaryKey,
                    IndexClustered,
                    clusteredIndexContainsPrimaryKey,
                    new List<ForeignKeyInfo>(),
                    new List<ForeignKeyInfo>(),
                    Indices
                );
        }
        public List<ColumnInfo> PrimaryKeyColumns => this.IndexPrimaryKey.Columns;

        public List<ColumnInfo> FastPrimaryKeyColumns =>
            this.IndexClustered is not null && this.ClusteredIndexContainsPrimaryKey
                ? this.IndexClustered.Columns
                : this.IndexPrimaryKey.Columns;

        private string? _GetNameQ;
        public string GetNameQ() => (_GetNameQ ??= $"[{this.Schema}].[{this.Name}]");

        public List<ColumnInfo> ColumnsWithRowversion {
            get {
                var result = new List<ColumnInfo>();
                result.AddRange(this.Columns);
                if (this.ColumnRowversion is not null) {
                    result.Add(this.ColumnRowversion);
                }
                return result;
            }
        }
    };
}
