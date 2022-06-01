namespace Brimborium.CodeGeneration.SQLRelated {
    public static class UtilitySQLInfo {
        public static Database GetDatabase(
            Microsoft.Data.SqlClient.SqlConnection connection
            ) {
            // using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            var serverConnection = new Microsoft.SqlServer.Management.Common.ServerConnection(connection);
            var server = new Microsoft.SqlServer.Management.Smo.Server(serverConnection);
            if (server is null) {
                throw new InvalidArgumentException($"server:{serverConnection.ServerInstance} not found!");
            }
            
            var database = server.Databases[connection.Database];
            if (database is null) {
                throw new InvalidArgumentException($"Database:{serverConnection.DatabaseName} not found!");
            }

            return database;
        }

        public static DatabaseInfo ExtractDatabaseSchema(
            Database database,
            Func<string, bool> precondition
            ) {
            var result = new DatabaseInfo();
            //
            // table
            //
            foreach (Table table in database.Tables) {
                IndexInfo? indexPrimaryKey = null;
                IndexInfo? indexClustered = null;

                var lstColumns = new List<ColumnInfo>();
                ColumnInfo? columnRowversion = null;
                {
                    foreach (Column column in table.Columns) {
                        if (column.DataType.SqlDataType == SqlDataType.Timestamp) {
                            columnRowversion = ColumnInfo.Create(column);
                        } else {
                            lstColumns.Add(ColumnInfo.Create(column));
                        }
                    }
                }
                var dctColumns = lstColumns.ToDictionary(c => c.Name, c => c, StringComparer.OrdinalIgnoreCase);

                var lstIndexes = new List<IndexInfo>();
                {
                    foreach (Microsoft.SqlServer.Management.Smo.Index index in table.Indexes) {
                        var indexInfo = IndexInfo.Create(index);
                        var lstIndexedColumn = index.IndexedColumns.Cast<IndexedColumn>()
                                .OrderBy(ic => ic.ID)
                                .Select(ic => (Position: ic.ID, Column: dctColumns[ic.Name]))
                                .ToList();
                        indexInfo.Columns.AddRange(lstIndexedColumn.Select(ic => ic.Column));
                        //
                        if (index.IsClustered) {
                            indexClustered = indexInfo;
                            lstIndexedColumn.ForEach(ic => ic.Column.ClusteredIndexPosition = ic.Position);
                        }
                        if (index.IndexKeyType == IndexKeyType.DriPrimaryKey) {
                            indexPrimaryKey = indexInfo;
                            lstIndexedColumn.ForEach(ic => ic.Column.PrimaryKeyIndexPosition = ic.Position);
                        }
                        lstIndexes.Add(indexInfo);
                    }
                }

                if (indexPrimaryKey is null) {
                    System.Console.WriteLine($"hint: no PrimaryKey {table.Schema}.{table.Name}");
                }
                if (columnRowversion is null) {
                    System.Console.WriteLine($"hint: no columnRowversion {table.Schema}.{table.Name}");
                }

                if (indexPrimaryKey is not null
                    //&& columnRowversion is not null
                    ) {
                    bool clusteredIndexContainsPrimaryKey;
                    if (indexClustered is null) {
                        clusteredIndexContainsPrimaryKey = false;
                    } else if (ReferenceEquals(indexClustered, indexPrimaryKey)) {
                        clusteredIndexContainsPrimaryKey = true;
                    } else {
                        clusteredIndexContainsPrimaryKey = indexPrimaryKey.Columns
                            .All(column => column.ClusteredIndexPosition >= 0
                            );
                    }
                    //
                    result.AddTable(TableInfo.Create(
                        table,
                        lstColumns,
                        columnRowversion,
                        indexPrimaryKey,
                        indexClustered,
                        clusteredIndexContainsPrimaryKey,
                        lstIndexes
                        ));
                } else {
                }
            }
            //
            // foreign key
            //
            foreach (Table table in database.Tables) {

                if (result.TableByName.TryGetValue(GetNameQ(table), out var tableInfo)) {
                    var dctColumns = tableInfo.Columns.ToDictionary(c => c.Name, c => c, StringComparer.OrdinalIgnoreCase);

                    foreach (ForeignKey foreignKey in table.ForeignKeys) {
                        if (result.TableByName.TryGetValue($"[{foreignKey.ReferencedTableSchema}].[{foreignKey.ReferencedTable}]", out var tableInfoReferenced)) {
                            var lstForeignKeyColumns = foreignKey.Columns.Cast<ForeignKeyColumn>()
                                .OrderBy(fkc => fkc.ID)
                                .Select(fkc => dctColumns[fkc.Name])
                                .ToList();
                            var referencedKeyName = foreignKey.ReferencedKey;
                            var indexInfoReferenced = tableInfoReferenced.Indices.Single(i => string.Equals(i.Name, referencedKeyName, StringComparison.OrdinalIgnoreCase));
                            var foreignKeyInfo = ForeignKeyInfo.Create(
                                    foreignKey,
                                    tableInfo,
                                    lstForeignKeyColumns,
                                    tableInfoReferenced,
                                    indexInfoReferenced
                                );
                            tableInfo.ForeignKeys.Add(foreignKeyInfo);
                            tableInfoReferenced.ForeignKeysReferenced.Add(foreignKeyInfo);
                        }
                    }
                }
            }
            //
            return result;
        }

        public static string GetNameQ(Table table) => $"[{table.Schema}].[{table.Name}]";


    }
}
