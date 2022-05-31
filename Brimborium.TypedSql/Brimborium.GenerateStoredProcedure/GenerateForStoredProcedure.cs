namespace Brimborium.GenerateStoredProcedure {
    public partial class GenerateForStoredProcedure {
        public static bool GenerateSql(
            string connectionString,
            string outputFolder,
            GenerateConfigurationBase cfg,
            Dictionary<string, string> templateVariables,
            bool isVerbose,
            bool isForce) {
            using var connection = new SqlConnection(connectionString);
            var serverConnection = new ServerConnection(connection);
            var server = new Server(serverConnection);
            if (server is null) {
                throw new InvalidArgumentException($"server:{serverConnection.ServerInstance} not found!");
            }

            var database = server.Databases[connection.Database];
            if (database is null) {
                throw new InvalidArgumentException($"Database:{serverConnection.DatabaseName} not found!");
            }
            var databaseInfo = ExtractDatabaseSchema(database);
            var config = cfg.Build(databaseInfo, isVerbose);
            templateVariables["ProjectRoot"] = outputFolder;
            templateVariables["Schema"] = "";
            templateVariables["Name"] = "";
            var di = new System.IO.DirectoryInfo(outputFolder);
            var lstFileContent = di.GetFiles("*.sql", System.IO.SearchOption.AllDirectories)
                .Select(fi => new FileContent(fi.FullName, System.IO.File.ReadAllText(fi.FullName)))
                .OrderBy(fi => fi.FileName)
                .ToList();
            /*
            foreach (var fc in lstFileContent) {
                System.Console.WriteLine(fc.FileName);
            }
            */
            var lstFileContentGenerated = new List<FileContent>();
            var lstFileContentReplacements = new List<FileContent>();
            foreach (var fc in lstFileContent) {
                if (ReplacementBindingExtension.ContainsReplace(fc.Content)) {
                    lstFileContentReplacements.Add(fc);
                } else {
                    lstFileContentGenerated.Add(fc);
                }
            }
            return GenerateSqlCode(config, templateVariables, lstFileContentGenerated, lstFileContentReplacements, WriteText, isVerbose, isForce);
        }

        public static bool GenerateSqlCode(
            ConfigurationBound config,
            Dictionary<string, string> templateVariables,
            List<FileContent> lstFileContentGenerated,
            List<FileContent> lstFileContentReplacements,
            Func<string, string, bool> writeText,
            bool isVerbose,
            bool isForce) {
            var result = false;
            var dctFileContent = lstFileContentReplacements.ToDictionary(fc => fc.FileName, StringComparer.OrdinalIgnoreCase);
            var lstFileContent = new List<FileContent>(lstFileContentGenerated.Count + lstFileContentReplacements.Count);
            lstFileContent.AddRange(lstFileContentGenerated);
            lstFileContent.AddRange(lstFileContentReplacements.Where(fc=>!fc.Content.Contains("-- Replace=SNIPPETS -")));

            System.Exception? lastError = null;
            var renderGenerator = new RenderGenerator(config.ReplacementBindings, templateVariables);
            foreach (var renderBinding in config.RenderBindings) {
                var boundVariables = new Dictionary<string, string>(templateVariables);
                var outputFilename = renderBinding.GetFilename(boundVariables);
                if (string.IsNullOrEmpty(outputFilename)) {
                    continue;
                }
                if (dctFileContent.ContainsKey(outputFilename)) {
                    System.Console.WriteLine($"skip: {outputFilename}");
                    continue;
                }
                var codeIdentity = renderBinding.GetCodeIdentity();
                if (!string.IsNullOrEmpty(codeIdentity)) {
                    var fileContentFound = lstFileContent.FirstOrDefault(fc => fc.Content.Contains(codeIdentity, StringComparison.OrdinalIgnoreCase));
                    if (fileContentFound is not null) {
                        if (isVerbose) {
                            System.Console.WriteLine($"redirect {outputFilename}->{fileContentFound.FileName}");
                        }
                        outputFilename = fileContentFound.FileName;
                    }
                }

                // System.Console.WriteLine(outputFilename);
                var fileContent = lstFileContentReplacements.FirstOrDefault(fc => string.Equals(fc.FileName, outputFilename, StringComparison.OrdinalIgnoreCase));
                if (fileContent is not null) {
                    lstFileContentReplacements.Remove(fileContent);
                }
                try {
                    var sbOutput = new StringBuilder();
                    var printCtxt = new PrintContext(sbOutput, boundVariables);
                    renderBinding.Render(printCtxt);
                    var (changed, content) = ReplacementBindingExtension.Replace(sbOutput.ToString(), renderGenerator.GetValue);
                    if (writeText(outputFilename, content)) {
                        Console.WriteLine($"changed: {outputFilename}");
                        result = true;
                    } else {
                        if (isVerbose) {
                            Console.WriteLine($"ok     : {outputFilename}");
                        }
                    }
                } catch (System.Exception error) {
                    lastError = error;
                    Console.WriteLine($"error     : {outputFilename}");
                    System.Console.Error.WriteLine(error.ToString());
                    Console.WriteLine($"error     : {outputFilename}");
                }
            }

            foreach (var fileContent in lstFileContentReplacements) {
                try {
                    var outputFilename = fileContent.FileName;
                    var (changed, content) = ReplacementBindingExtension.Replace(fileContent.Content, renderGenerator.GetValue);
                    if (changed && writeText(outputFilename, content)) {
                        Console.WriteLine($"changed: {outputFilename}");
                        result = true;
                    } else {
                        if (isVerbose) {
                            Console.WriteLine($"ok     : {outputFilename}");
                        }
                    }
                } catch (System.Exception error) {
                    lastError = error;
                    Console.WriteLine($"error     : {fileContent.FileName}");
                    System.Console.Error.WriteLine(error.ToString());
                    Console.WriteLine($"error     : {fileContent.FileName}");
                }
                if (lastError is not null) {
                    throw lastError;
                }
            }
            return result;
        }

        public static DatabaseInfo ExtractDatabaseSchema(Database database) {
            var result = new DatabaseInfo();
            var lstTables = result.Tables;
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
                    lstTables.Add(TableInfo.Create(
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
            var dctTables = lstTables.ToDictionary(ti => ti.GetNameQ(), ti => ti, StringComparer.OrdinalIgnoreCase);
            //
            // foreign key
            //
            foreach (Table table in database.Tables) {
                if (dctTables.TryGetValue($"[{table.Schema}].[{table.Name}]", out var tableInfo)) {
                    var dctColumns = tableInfo.Columns.ToDictionary(c => c.Name, c => c, StringComparer.OrdinalIgnoreCase);

                    foreach (ForeignKey foreignKey in table.ForeignKeys) {
                        if (dctTables.TryGetValue($"[{foreignKey.ReferencedTableSchema}].[{foreignKey.ReferencedTable}]", out var tableInfoReferenced)) {
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

#if weichei
        public static string GetFilename(string filePattern, Dictionary<string, string> boundVariables) {
            var result = new StringBuilder();
            int iPosPrev = 0;
            while (iPosPrev < filePattern.Length) {
                var iPosStart = filePattern.IndexOf('[', iPosPrev);
                if (iPosStart < 0) {
                    break;
                }
                int iPosEnd = filePattern.IndexOf(']', iPosStart);
                //
                if (iPosPrev < iPosStart) {
                    var constPart = filePattern.Substring(iPosPrev, iPosStart - iPosPrev);
                    result.Append(constPart);
                }
                //
                {
                    var namePart = filePattern.Substring(iPosStart + 1, iPosEnd - iPosStart - 1);
                    if (boundVariables.TryGetValue(namePart, out var value)) {
                        result.Append(value);
                    }
                }
                //
                iPosPrev = iPosEnd + 1;
            }
            if (iPosPrev < filePattern.Length) {
                var constPart = filePattern.Substring(iPosPrev, filePattern.Length - iPosPrev);
                result.Append(constPart);
            }
            var outputFolder = boundVariables["ProjectRoot"]!;
            return System.IO.Path.Combine(
                outputFolder,
                result.ToString()
            );
        }
#endif

        public static bool WriteText(string fileName, string fileContent) {
#if true
            if (System.IO.File.Exists(fileName)) {
                var oldContent = System.IO.File.ReadAllText(fileName);
                if (string.CompareOrdinal(oldContent, fileContent) == 0) {
                    return false;
                }
            } else {
                var directoryName = System.IO.Path.GetDirectoryName(fileName);
                if (!string.IsNullOrEmpty(directoryName)) {
                    var di = new System.IO.DirectoryInfo(directoryName);
                    if (!di.Exists) {
                        di.Create();
                    }
                }
            }
            System.IO.File.WriteAllText(fileName, fileContent);
            return true;
#else
            System.Console.WriteLine(fileName);
            System.Console.WriteLine(fileContent);
            return true;
#endif
        }


        //StoredProcedures

        public static string GetStoredProceduresFileName(
            string outputFolder,
            string objectType,
            string schema, string name, string suffix) {
            return System.IO.Path.Combine(
                outputFolder,
                $@"src\Solvin.OneTS.Database\{schema}\{objectType}\{schema}.{name}{suffix}.sql"
                );
        }
    }
}
