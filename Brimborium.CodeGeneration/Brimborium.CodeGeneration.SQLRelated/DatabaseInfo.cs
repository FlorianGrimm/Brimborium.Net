namespace Brimborium.CodeGeneration.SQLRelated {
    public sealed record DatabaseInfo() {
        public HashSet<TableInfo> Tables { get; init; } = new HashSet<TableInfo>();
        public SortedDictionary<string, TableInfo> TableByName { get; init; } = new SortedDictionary<string, TableInfo>(StringComparer.InvariantCultureIgnoreCase);
        public HashSet<ForeignKeyInfo> ForeignKeys { get; init; } = new HashSet<ForeignKeyInfo>();
        public SortedDictionary<string, ForeignKeyInfo> ForeignKeyByName { get; init; } = new SortedDictionary<string, ForeignKeyInfo>(StringComparer.InvariantCultureIgnoreCase);

        public void AddTable(TableInfo tableInfo) {
            if (this.TableByName.TryAdd(tableInfo.GetNameQ(), tableInfo)) {
                this.Tables.Add(tableInfo);
            }

        }

        public void AddForeignKey(ForeignKeyInfo foreignKeyInfo) {
            if (this.ForeignKeyByName.TryAdd(foreignKeyInfo.GetNameQ(), foreignKeyInfo)) {
                this.ForeignKeys.Add(foreignKeyInfo);
            }
        }

        public DatabaseInfo Filter(Func<TableInfo, bool> precondition) {
            var result = new DatabaseInfo();
            foreach (var tableInfo in this.Tables.Where(precondition)) {
                result.AddTable(tableInfo);
            }
            foreach (var foreignKey in result.Tables.SelectMany(t => t.ForeignKeys)) {

                if (result.Tables.Contains(foreignKey.TableInfo)
                    && result.Tables.Contains(foreignKey.TableInfoReferenced)) {
                    result.AddForeignKey(foreignKey);
                }

            }
            return result;
        }
    };
}
