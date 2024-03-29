﻿namespace Brimborium.CodeGeneration.SQLRelated {
    public sealed record ForeignKeyInfo(
        string Name,
        ForeignKey ForeignKey,
        TableInfo TableInfo,
        List<ColumnInfo> ForeignKeyColumns,
        TableInfo TableInfoReferenced,
        IndexInfo IndexReferenced
        ) {
        public string GetShortName() {
            var result = this.ForeignKey.Name;
            //FK_dbo_Account_User
            if (result.StartsWith("FK_")) { result = result.Substring(3); }
            if (result.StartsWith($"{this.TableInfo.Schema}_")) { result = result.Substring(this.TableInfo.Schema.Length + 1); }
            if (result.StartsWith($"{this.TableInfo.Name}_")) { result = result.Substring(this.TableInfo.Name.Length + 1); }
            return result;
        }
        public List<ColumnInfo> ForeignKeyColumnsReferenced
            => IndexReferenced.Columns;

        public List<(ColumnInfo FKC, ColumnInfo RefC)> PairedColumns
            => ForeignKeyColumns.Zip(IndexReferenced.Columns, (FKC, RefC) => (FKC, RefC)).ToList();

        private string? _GetNameQ;
        public string GetNameQ() => (this._GetNameQ ??= $"[{this.TableInfo.Schema}].[{this.Name}]");

        public static ForeignKeyInfo Create(
            ForeignKey ForeignKey,
            TableInfo TableInfo,
            List<ColumnInfo> ForeignKeyColumns,
            TableInfo TableInfoReferenced,
            IndexInfo IndexReferenced
            ) {
            return new ForeignKeyInfo(
                Name: ForeignKey.Name,
                ForeignKey: ForeignKey,
                TableInfo: TableInfo,
                ForeignKeyColumns: ForeignKeyColumns,
                TableInfoReferenced: TableInfoReferenced,
                IndexReferenced: IndexReferenced
                );
        }
    }
}
