using Brimborium.GenerateStoredProcedure;

using System.Collections.Generic;
using System.Linq;

namespace Brimborium.TestGenerateStoredProcedure {
    public class GenerateConfiguration : Brimborium.GenerateStoredProcedure.Configuration {
        public readonly RenderTemplate<TableInfo> SelectPKTempate;
        
        public readonly RenderTemplate<TableInfo> SelectAtTimeTempate;
        
        public readonly RenderTemplate<ForeignKeyInfo> SelectByReferencedPKTempate;

        public record TableDataHistory(
            TableInfo TableData,
            TableInfo TableHistory,
            List<(ColumnInfo columnData, ColumnInfo columnHistory)> ColumnPairs);

        public record TableDataFK(
            TableInfo TableData,
            List<ForeignKeyInfo> ForeignKeyReferenced);

        public readonly RenderTemplate<TableDataHistory> UpdateTempate;

        public readonly RenderTemplate<TableDataHistory> DeletePKTempate;
        
        public readonly List<ReplacementTemplate<TableInfo>> ReplacementTableTemplates;

        public GenerateConfiguration() {
            this.ReplacementTableTemplates = new List<ReplacementTemplate<TableInfo>>();
            var KT = new KnownTemplates();

            this.SelectPKTempate = new RenderTemplate<TableInfo>(
                FileNameFn: RenderTemplateExtentsions.GetFileNameBind<TableInfo>(@"[Schema]\StoredProcedures\[Schema].[Name]SelectPK.sql"),
                Render: (TableInfo data, PrintContext ctxt) => {
                    KnownTemplates.SqlCreateProcedure<TableInfo>(
                        data,
                        ctxt,
                        schema: data.Table.Schema,
                        name: $"{data.Table.Name}SelectPK",
                        parameter: (data, ctxt) => {
                            ctxt.RenderTemplate(
                                data.PrimaryKeyColumns
                                , KT.ColumnsAsParameter);
                        },
                        bodyBlock: (data, ctxt) => {
                            KnownTemplates.SqlSelect(
                                data,
                                ctxt,
                                top: null,
                                columnsBlock: (data, ctxt) => {
                                    ctxt.AppendList(
                                        data.Columns,
                                        (column, ctxt) => {
                                            ctxt.AppendPartsLine(
                                                column.GetNameQ(), ","
                                                );
                                        });
                                    ctxt.AppendLine(
                                        $"{data.ColumnRowversion.GetNameQ()} = CAST({data.ColumnRowversion.GetNameQ()} as BIGINT)");
                                },
                                fromBlock: (data, ctxt) => {
                                    ctxt.AppendLine(data.GetNameQ());
                                },
                                whereBlock: (data, ctxt) => {
                                    ctxt.AppendList(
                                        data.PrimaryKeyColumns,
                                        (column, ctxt) => {
                                            ctxt.AppendPartsLine(
                                                ctxt.IfNotFirst(" AND "),
                                                "(",
                                                column.GetNamePrefixed("@"), " = ", column.GetNameQ(),
                                                ")"
                                                );
                                        });
                                });
                        });
                }
            );

            this.SelectAtTimeTempate = new RenderTemplate<TableInfo>(
               FileNameFn: RenderTemplateExtentsions.GetFileNameBind<TableInfo>(@"[Schema]\StoredProcedures\[Schema].[Name]SelectAtTime.sql"),
               Render: (TableInfo data, PrintContext ctxt) => {
                   //List<ColumnInfo> parameters = new List<ColumnInfo>();
                   //if (new string[] { "ExternalSourceId" }.SequenceEqual(data.PrimaryKeyColumns.Select(c => c.Name))) {
                   //    // skip
                   //} else {
                   //    parameters.AddRange(data.Columns.Where(c => c.Name == "ExternalSourceId"));
                   //}
                   KnownTemplates.SqlCreateProcedure<TableInfo>(
                       data,
                       ctxt,
                       schema: data.Table.Schema,
                       name: $"{data.Table.Name}SelectAtTime",
                       parameter: (data, ctxt) => {
                           ctxt.AppendLine("@PointInTime DATETIME");

                           //ctxt.RenderTemplate(
                           //    parameters
                           //    , kt.ColumnsAsParameter
                           //    );
                       },
                       bodyBlock: (data, ctxt) => {
                           KnownTemplates.SqlSelect(
                               data,
                               ctxt,
                               top: null,
                               columnsBlock: (data, ctxt) => {
                                   ctxt.AppendList(
                                       data.Columns,
                                       (column, ctxt) => {
                                           ctxt.AppendPartsLine(
                                               column.GetNameQ(), ","
                                               );
                                       });
                                   ctxt.AppendLine(
                                       $"{data.ColumnRowversion.GetNameQ()} = CAST({data.ColumnRowversion.GetNameQ()} as BIGINT)");
                               },
                               fromBlock: (data, ctxt) => {
                                   ctxt.AppendLine(data.GetNameQ());
                               },
                               whereBlock: (data, ctxt) => {
                                   ctxt.Append("([ValidFrom] <= @PointInTime) AND (@PointInTime < [ValidTo])");

                               }
                               //whereBlock: parameters.Any() ? ((data, ctxt) => {
                               //    ctxt.AppendList(
                               //        data.PrimaryKeyColumns,
                               //        (column, ctxt) => {
                               //            ctxt.AppendPartsLine(
                               //                ctxt.IfNotFirst(" AND "),
                               //                "(",
                               //                column.GetNamePrefixed("@"), " = ", column.GetNameQ(),
                               //                ")"
                               //                );
                               //        });
                               //}) : null
                               );
                       });
               }
            );


            this.SelectByReferencedPKTempate = new RenderTemplate<ForeignKeyInfo>(
                FileNameFn: (ForeignKeyInfo foreignKey, Dictionary<string, string> boundVariables)
                => RenderTemplateExtentsions.GetAbsoluteFilename(
                    $@"{foreignKey.TableInfo.Schema}\StoredProcedures\{foreignKey.TableInfo.Schema}.{foreignKey.TableInfo.Name}SelectByFK{foreignKey.GetShortName()}.sql",
                    boundVariables)
                ,
                Render: (ForeignKeyInfo foreignKey, PrintContext ctxt) => {
                    KnownTemplates.SqlCreateProcedure<ForeignKeyInfo>(
                        foreignKey,
                        ctxt,
                        schema: foreignKey.TableInfo.Schema,
                        name: $"{foreignKey.TableInfo.Name}SelectByFK{foreignKey.GetShortName()}",
                        parameter: (data, ctxt) => {
                            ctxt.RenderTemplate(
                                data.ForeignKeyColumnsReferenced
                                , KT.ColumnsAsParameter);
                        },
                        bodyBlock: (data, ctxt) => {
                            KnownTemplates.SqlSelect(
                                data,
                                ctxt,
                                top: null,
                                columnsBlock: (data, ctxt) => {
                                    KT.Columns.Render(data.TableInfo.Columns, ctxt);
                                    KT.ColumnRowversion.Render(data.TableInfo, ctxt);
                                },
                                fromBlock: (data, ctxt) => {
                                    ctxt.AppendLine(data.TableInfo.GetNameQ());
                                },
                                whereBlock: (data, ctxt) => {
                                    ctxt.AppendList(
                                        data.PairedColumns,
                                        (pairedColumn, ctxt) => {
                                            ctxt.AppendPartsLine(
                                                ctxt.IfNotFirst(" AND "),
                                                "(",
                                                pairedColumn.FKC.GetNamePrefixed("@"), " = ", pairedColumn.RefC.GetNameQ(),
                                                ")"
                                                );
                                        });
                                });
                        });
                }
            );

            this.UpdateTempate = new RenderTemplate<TableDataHistory>(
                FileNameFn: RenderTemplateExtentsions.GetFileNameBind<TableDataHistory>(@"[Schema]\StoredProcedures\[Schema].[Name]Upsert.sql"),
                Render: (TableDataHistory data, PrintContext ctxt) => {

                    KnownTemplates.SqlCreateProcedure<TableDataHistory>(
                        data,
                        ctxt,
                        schema: data.TableData.Table.Schema,
                        name: $"{data.TableData.Table.Name}Upsert",
                        parameter: (data, ctxt) => {
                            ctxt.RenderTemplate(
                                (columns: data.TableData.Columns, columnRowVersion: data.TableData.ColumnRowversion),
                                KT.ColumnsAsParameterWithRowVersion
                                );
                        },
                        bodyBlock: (data, ctxt) => {
                            ctxt.RenderTemplate(
                                (columns: data.TableData.Columns, columnRowVersion: data.TableData.ColumnRowversion, prefix: "@Current"),
                                KT.ColumnsAsDeclareParameterWithRowVersion
                                );
                            ctxt.AppendLine("DECLARE @ResultValue INT;");
                            ctxt.AppendLine("");
                            KnownTemplates.SqlIf(
                                data,
                                ctxt,
                                condition: (data, ctxt) => {
                                    ctxt.Append(data.TableData.ColumnRowversion.GetNamePrefixed("@Current")).Append(" > 0");
                                },
                                thenBlock: (data, ctxt) => {
                                    KnownTemplates.SqlSelect(
                                        data.TableData,
                                        ctxt,
                                        top: 1,
                                        columnsBlock: (data, ctxt) => {
                                            ctxt.AppendList(
                                                data.Columns,
                                                (column, ctxt) => {
                                                    ctxt.AppendPartsLine(
                                                        column.GetNamePrefixed("@Current"), " = ", column.GetNameQ(), ","
                                                        );
                                                });
                                            ctxt.AppendPartsLine(
                                                data.ColumnRowversion.GetNamePrefixed("@Current"),
                                                " = CAST(",
                                                data.ColumnRowversion.GetNameQ(),
                                                " as BIGINT)");
                                        },
                                        fromBlock: (data, ctxt) => {
                                            ctxt.AppendLine(data.GetNameQ());
                                        },
                                        whereBlock: (data, ctxt) => {
                                            ctxt.AppendList(
                                                data.PrimaryKeyColumns, // TODO FastPrimaryKeyColumns,
                                                (column, ctxt) => {
                                                    ctxt.AppendPartsLine(
                                                        ctxt.IfNotFirst(" AND "),
                                                        "(",
                                                        column.GetNamePrefixed("@"), " = ", column.GetNameQ(),
                                                        ")"
                                                        );
                                                });
                                        });
                                },
                                elseBlock: (data, ctxt) => {
                                    KnownTemplates.SqlSelect(
                                        data.TableData,
                                        ctxt,
                                        top: 1,
                                        columnsBlock: (data, ctxt) => {
                                            ctxt.AppendPartsLine(
                                                data.ColumnRowversion.GetNamePrefixed("@Current"),
                                                " = CAST(",
                                                data.ColumnRowversion.GetNameQ(),
                                                " as BIGINT)");
                                        },
                                        fromBlock: (data, ctxt) => {
                                            ctxt.AppendLine(data.GetNameQ());
                                        },
                                        whereBlock: (data, ctxt) => {
                                            ctxt.AppendList(
                                                data.PrimaryKeyColumns, // TODO FastPrimaryKeyColumns,
                                                (column, ctxt) => {
                                                    ctxt.AppendPartsLine(
                                                        ctxt.IfNotFirst(" AND "),
                                                        "(",
                                                        column.GetNamePrefixed("@"), " = ", column.GetNameQ(),
                                                        ")"
                                                        );
                                                });
                                        });
                                });
                            KnownTemplates.SqlIf(
                                data,
                                ctxt,
                                condition: (data, ctxt) => {
                                    ctxt.Append(
                                        KnownTemplates.SqlIsNull(
                                                data.TableData.ColumnRowversion.GetNamePrefixed("@Current")
                                            )
                                        );
                                },
                                thenBlock: (data, ctxt) => {

                                    KnownTemplates.SqlInsertValues(
                                        data,
                                        ctxt,
                                        target: data.TableData.GetNameQ(),
                                        nameBlock: (data, ctxt) => {
                                            ctxt.AppendList(
                                                data.TableData.Columns.Where(c => !c.Identity).ToList(),
                                                (column, ctxt) => {
                                                    ctxt.AppendPartsLine(column.GetNameQ(), ctxt.IfNotLast(","));
                                                }
                                                );
                                        },
                                        valuesBlock: (data, ctxt) => {
                                            ctxt.AppendList(
                                                data.TableData.Columns.Where(c => !c.Identity).ToList(),
                                                (column, ctxt) => {
                                                    ctxt.AppendPartsLine(column.GetNamePrefixed("@"), ctxt.IfNotLast(","));
                                                });
                                        });
                                    ctxt.AppendLine("SET @ResultValue = 1; /* Inserted */");
                                    /* History */
                                    KnownTemplates.SqlInsertValues(
                                        data,
                                        ctxt,
                                        target: data.TableHistory.GetNameQ(),
                                        nameBlock: (data, ctxt) => {
                                            ctxt.AppendList(
                                                data.ColumnPairs,
                                                (columnPair, ctxt) => {
                                                    ctxt.AppendPartsLine(
                                                        columnPair.columnHistory.GetNameQ(),
                                                        ","
                                                        );
                                                }
                                                );
                                            ctxt.AppendLine("[ValidFrom],");
                                            ctxt.AppendLine("[ValidTo]");
                                        },
                                        valuesBlock: (data, ctxt) => {
                                            ctxt.AppendList(
                                                data.ColumnPairs,
                                                (columnPair, ctxt) => {
                                                    ctxt.AppendPartsLine(
                                                        columnPair.columnData.GetNamePrefixed("@"),
                                                        ","
                                                        );
                                                }
                                                );
                                            ctxt.AppendLine("@ModifiedAt,");
                                            ctxt.AppendLine("CAST('3141-05-09T00:00:00Z' as datetimeoffset)");
                                        }
                                        );
                                },
                                elseBlock: (data, ctxt) => {
                                    KnownTemplates.SqlIf(
                                        data,
                                        ctxt,
                                        condition: (data, ctxt) => {
                                            var crv = data.TableData.ColumnRowversion.GetNamePrefixed("@");
                                            var crvCurrent = data.TableData.ColumnRowversion.GetNamePrefixed("@Current");
                                            ctxt.AppendPartsLine("(", crv, " <= 0)");
                                            ctxt.AppendPartsLine("OR ((0 < ", crv, ") AND (", crv, " = ", crvCurrent, "))");
                                        },
                                        thenBlock: (data, ctxt) => {
                                            KnownTemplates.SqlIf(
                                               data,
                                               ctxt,
                                               condition: (data, ctxt) => {
                                                   ctxt.AppendLine("EXISTS(");
                                                   var ctxt2 = ctxt.GetIndented();
                                                   var ctxt3 = ctxt2.GetIndented();
                                                   var columnsIncludedInCompare = data.TableData.Columns.Where(
                                                       c => (c.ExtraInfo["ExcludeFromCompare"] switch {
                                                           true => false,
                                                           _ => true
                                                       })).ToList();
                                                   ctxt2.AppendLine("SELECT");
                                                   ctxt3.AppendList(
                                                       columnsIncludedInCompare,
                                                       (column, ctxt) => {
                                                           ctxt.AppendPartsLine(column.GetNamePrefixed("@"), ctxt.IfNotLast(","));
                                                       });
                                                   ctxt2.AppendLine("EXCEPT");
                                                   ctxt2.AppendLine("SELECT");
                                                   ctxt3.AppendList(
                                                       columnsIncludedInCompare,
                                                       (column, ctxt) => {
                                                           ctxt.AppendPartsLine(column.GetNamePrefixed("@Current"), ctxt.IfNotLast(","));
                                                       });
                                                   ctxt.Append(")");
                                               },
                                               thenBlock: (data, ctxt) => {
                                                   KnownTemplates.SqlUpdate(
                                                       data,
                                                       ctxt,
                                                       top: 1,
                                                       target: data.TableData.GetNameQ(),
                                                       setBlock: (data, ctxt) => {
                                                           ctxt.AppendList(
                                                               data.TableData.Columns.Where(column => column.PrimaryKeyIndexPosition < 0),
                                                               (column, ctxt) => {
                                                                   ctxt.AppendPartsLine(
                                                                       column.GetNameQ(),
                                                                       " = ",
                                                                       column.GetNamePrefixed("@"),
                                                                       ctxt.IfNotLast(","));
                                                               });
                                                       },
                                                       whereBlock: (data, ctxt) => {
                                                           ctxt.AppendList(data.TableData.PrimaryKeyColumns, (column, ctxt) => {
                                                               ctxt.AppendPartsLine(
                                                                   ctxt.IfNotFirst(" AND "),
                                                                   "(",
                                                                   column.GetNameQ(),
                                                                   " = ",
                                                                   column.GetNamePrefixed("@"),
                                                                   ")"
                                                                   );
                                                           });
                                                       });
                                                   ctxt.AppendLine("SET @ResultValue = 2; /* Updated */");
                                                   /* History */
                                                   KnownTemplates.SqlUpdate(
                                                       data,
                                                       ctxt,
                                                       top: 1,
                                                       target: data.TableHistory.GetNameQ(),
                                                       setBlock: (data, ctxt) => {
                                                           ctxt.AppendLine("[ValidTo] = @ModifiedAt");
                                                       },
                                                       whereBlock: (data, ctxt) => {
                                                           ctxt.AppendLine("([ValidTo] = CAST('3141-05-09T00:00:00Z' as datetimeoffset))");
                                                           ctxt.AppendList(
                                                                data.TableHistory.IndexPrimaryKey.Columns
                                                                    .Where(c =>
                                                                        !(string.Equals(c.Name, "ValidTo")
                                                                        || string.Equals(c.Name, "ValidFrom"))
                                                                    ),
                                                                (column, ctxt) => {
                                                                    //ctxt.Append(ctxt.IfNotFirst("AND "));
                                                                    ctxt.AppendPartsLine("AND (", column.GetNameQ(), " = ", column.GetNamePrefixed("@"), ")");
                                                                });
                                                       }
                                                       );
                                                   KnownTemplates.SqlInsertValues(
                                                       data,
                                                       ctxt,
                                                       target: data.TableHistory.GetNameQ(),
                                                       nameBlock: (data, ctxt) => {
                                                           ctxt.AppendList(
                                                               data.ColumnPairs,
                                                               (columnPair, ctxt) => {
                                                                   ctxt.AppendPartsLine(
                                                                       columnPair.columnHistory.GetNameQ(),
                                                                       ","
                                                                       );
                                                               }
                                                               );
                                                           ctxt.AppendLine("[ValidFrom],");
                                                           ctxt.AppendLine("[ValidTo]");
                                                       },
                                                       valuesBlock: (data, ctxt) => {
                                                           ctxt.AppendList(
                                                               data.ColumnPairs,
                                                               (columnPair, ctxt) => {
                                                                   ctxt.AppendPartsLine(
                                                                       columnPair.columnData.GetNamePrefixed("@"),
                                                                       ","
                                                                       );
                                                               }
                                                               );
                                                           ctxt.AppendLine("@ModifiedAt,");
                                                           ctxt.AppendLine("CAST('3141-05-09T00:00:00Z' as datetimeoffset)");
                                                       }
                                                       );

                                               },
                                               elseBlock: (data, ctxt) => {
                                                   ctxt.AppendLine("SET @ResultValue = 0; /* NoNeedToUpdate */");
                                               });
                                        },
                                        elseBlock: (data, ctxt) => {
                                            ctxt.AppendLine("SET @ResultValue = -1 /* RowVersionMismatch */;");
                                        });
                                });
                            KnownTemplates.SqlSelect(
                                data,
                                ctxt,
                                top: 1,
                                columnsBlock: (data, ctxt) => {
                                    ctxt.AppendList(
                                        data.TableData.Columns,
                                        (column, ctxt) => {
                                            ctxt.AppendPartsLine(
                                                column.GetNameQ(), ","
                                                );
                                        });
                                    ctxt.AppendLine(
                                        $"{data.TableData.ColumnRowversion.GetNameQ()} = CAST({data.TableData.ColumnRowversion.GetNameQ()} as BIGINT)");
                                },
                                fromBlock: (data, ctxt) => {
                                    ctxt.AppendLine(data.TableData.GetNameQ());
                                },
                                whereBlock: (data, ctxt) => {
                                    ctxt.AppendList(
                                        data.TableData.PrimaryKeyColumns,
                                        (column, ctxt) => {
                                            ctxt.AppendPartsLine(
                                                ctxt.IfNotFirst(" AND "),
                                                "(",
                                                column.GetNamePrefixed("@"), " = ", column.GetNameQ(),
                                                ")"
                                                );
                                        });
                                });
                            ctxt.AppendLine("SELECT ResultValue = @ResultValue, Message='';");
                        }
                    );
                });

            this.DeletePKTempate = new RenderTemplate<TableDataHistory>(
                FileNameFn: RenderTemplateExtentsions.GetFileNameBind<TableDataHistory>(@"[Schema]\StoredProcedures\[Schema].[Name]DeletePK.sql"),
                Render: (TableDataHistory data, PrintContext ctxt) => {
                    KnownTemplates.SqlCreateProcedure<TableDataHistory>(
                        data,
                        ctxt,
                        schema: data.TableData.Table.Schema,
                        name: $"{data.TableData.Table.Name}DeletePK",
                        parameter: (data, ctxt) => {
                            //ctxt.RenderTemplate(
                            //    (columns: data.TableData.PrimaryKeyColumns, columnRowVersion: data.TableData.ColumnRowversion),
                            //    KT.ColumnsAsParameterWithRowVersion
                            //    );
                            ctxt.AppendList(data.TableData.PrimaryKeyColumns, (column, ctxt) => {
                                ctxt.AppendPartsLine(
                                    column.GetNamePrefixed("@"),
                                    " ",
                                    column.GetParameterSqlDataType(),
                                    ","
                                    );

                            });

                            ctxt.AppendPartsLine("@ActivityId uniqueidentifier,");
                            ctxt.AppendPartsLine("@ModifiedAt datetimeoffset,");
                            if (data.TableData.ColumnRowversion is not null) {
                                ctxt.AppendPartsLine(
                                        data.TableData.ColumnRowversion.GetNamePrefixed("@"),
                                        " ",
                                        data.TableData.ColumnRowversion.GetParameterSqlDataType()
                                        );
                            }
                        },
                        bodyBlock: (data, ctxt) => {
                            ctxt.AppendLine("DECLARE @Result AS TABLE (");
                            ctxt.GetIndented().AppendList(
                                data.TableData.PrimaryKeyColumns,
                                (column, ctxt) => {
                                    var sqlDataType = column.GetSqlDataType();
                                    var name = column.GetNameQ();
                                    ctxt.AppendPartsLine(name, " ", sqlDataType, ctxt.IfNotLast(","));
                                });
                            ctxt.AppendLine(");");

                            ctxt.AppendLine("");

                            ctxt.AppendLine($"DELETE FROM {data.TableData.GetNameQ()}");
                            var ctxtIndented1 = ctxt.GetIndented();
                            var ctxtIndented2 = ctxtIndented1.GetIndented();
                            ctxtIndented1.AppendLine("OUTPUT");
                            ctxtIndented2.AppendList(
                                data.TableData.PrimaryKeyColumns,
                                (column, ctxt) => {
                                    ctxt.AppendPartsLine(
                                        "DELETED.", column.GetNameQ(), ctxt.IfNotLast(",")
                                        );
                                });
                            ctxtIndented1.AppendLine("INTO @Result");

                            ctxtIndented1.AppendList(
                                data.TableData.PrimaryKeyColumns,
                                (column, ctxt) => {
                                    ctxt.AppendPartsLine(
                                        ctxt.SwitchFirst("WHERE ", "    AND "),
                                        "(",
                                        column.GetNamePrefixed("@"), " = ", column.GetNameQ(),
                                        ")"
                                        );
                                });
                            ctxtIndented1.AppendLine(";");

                            ctxt.AppendLine("");

                            KnownTemplates.SqlIf(
                                data,
                                ctxt,
                                condition: (data, ctxt) => {
                                    var ctxtIndented1 = ctxt.GetIndented();

                                    ctxt.AppendLine("EXISTS(");
                                    ctxt.AppendLine("SELECT");
                                    ctxtIndented1.AppendList(
                                        data.TableData.PrimaryKeyColumns,
                                        (column, ctxt) => {
                                            ctxt.AppendPartsLine(
                                                column.GetNameQ(), ctxt.IfNotLast(",")
                                                );
                                        });
                                    ctxtIndented1.AppendLine("FROM @Result");
                                    ctxt.AppendLine(")");
                                },
                                thenBlock: (data, ctxt) => {
                                    KnownTemplates.SqlUpdate(
                                        data,
                                        ctxt,
                                        top: 1,
                                        target: data.TableHistory.GetNameQ(),
                                        setBlock: (data, ctxt) => {
                                            ctxt.AppendLine("[ValidTo] = @ModifiedAt");
                                        },
                                        whereBlock: (data, ctxt) => {
                                            var ctxtIndented1 = ctxt.GetIndented();

                                            ctxtIndented1.AppendLine("([ActivityId] = @ActivityId)");
                                            ctxtIndented1.AppendLine("AND ([ValidTo] = CAST('3141-05-09T00:00:00Z' as datetimeoffset))");
                                            ctxtIndented1.AppendList(
                                                data.TableData.PrimaryKeyColumns,
                                                (column, ctxt) => {
                                                    ctxt.AppendPartsLine(
                                                        "    AND ",
                                                        "(",
                                                        column.GetNamePrefixed("@"), " = ", column.GetNameQ(),
                                                        ")"
                                                        );
                                                });
                                        }
                                        );
                                }
                                );

                            ctxt.AppendLine("SELECT");
                            ctxtIndented1.AppendList(
                                data.TableData.PrimaryKeyColumns,
                                (column, ctxt) => {
                                    ctxt.AppendPartsLine(
                                        column.GetNameQ(), ctxt.IfNotLast(",")
                                        );
                                });
                            ctxtIndented1.AppendLine("FROM @Result");
                            ctxtIndented1.AppendLine(";");

                        });
                }
            );
            //
        }

        public override ConfigurationBound Build(DatabaseInfo databaseInfo) {
            var result = new ConfigurationBound();

            var hsExcludeFromCompare = new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            hsExcludeFromCompare.Add("ActivityId");
            hsExcludeFromCompare.Add("CreatedAt");
            hsExcludeFromCompare.Add("ModifiedAt");
            hsExcludeFromCompare.Add("SerialVersion");

            foreach (var t in databaseInfo.Tables.Where(
                t => !string.Equals(t.Name, "Activity")
                    && !t.Name.EndsWith("History")
                )) {
                foreach (var column in t.Columns) {
                    var excludeFromCompare = hsExcludeFromCompare.Contains(column.Name);
                    column.ExtraInfo["ExcludeFromCompare"] = excludeFromCompare;
                }
            }

            foreach (var tableInfo in databaseInfo.Tables) {
                if (tableInfo.ColumnRowversion is not null) {
                    tableInfo.ColumnRowversion.ParameterSqlDataType = "bigint";
                }
                //foreach (var columnInfo in tableInfo.Columns) {
                //    if (columnInfo.Column.DataType.SqlDataType == Microsoft.SqlServer.Management.Smo.SqlDataType.Timestamp) {
                //        columnInfo.ParameterSqlDataType = "bigint";
                //    }
                //}
            }


            var tablesInsertOnly = databaseInfo.Tables.Where(
                t => IsActivityTable(t)
                ).ToList();

            var tablesHistory = databaseInfo.Tables.Where(
                t => !IsActivityTable(t) && IsAHistoryTable(t)
                ).ToList();
            /* var dictTablesHistory = tablesHistory.ToDictionary(t => $"{t.Schema}.{t.Name}"); */

            var tablesUpdate = databaseInfo.Tables.Where(
                t => !IsActivityTable(t) && !IsAHistoryTable(t)
                ).ToList();

            var tablesUpdatePaired = tablesUpdate.Join(
                    tablesHistory,
                    o =>  o.Name,
                    i =>  i.Name.EndsWith("History")?i.Name.Substring(0, i.Name.Length - 7):null,
                    (tableInfoData, tableInfoHistory) => new TableDataHistory(
                        tableInfoData,
                        tableInfoHistory,
                        tableInfoData.Columns.Join(
                            tableInfoHistory.Columns,
                            o => o.Name,
                            i => i.Name,
                            (o, i) => (columnData: o, columnHistory: i)
                            ).ToList()
                    )
                ).ToList();

            var tablesNotHistory = databaseInfo.Tables.Where(
                t => !IsAHistoryTable(t)
                ).ToList();

            var tablesDataWithFKReferenced = databaseInfo.Tables.Select(
                    tableInfo => tableInfo.ForeignKeysReferenced.Where(fk => !IsADataTable(fk.TableInfo))
                    .OrderBy(t => t.TableInfo.GetNameQ())
                    .ToList()
                ).Where(
                    fks => (fks.Count > 0)
                ).Select(
                    fks => new TableDataFK(fks[0].TableInfoReferenced, fks)
                ).ToList();


            result.AddRenderBindings(
                    "SelectPKTempate",
                    tablesNotHistory
                    .Select(tableInfo => new TableBinding(tableInfo, this.SelectPKTempate)));


            //result.RenderBindings.AddRange(
            //        databaseInfo.Tables.Where(t => t.Name == "ExternalSource")
            //        .Select(tableInfo => new TableBinding(tableInfo, this.SelectAtTimeTempate))
            //    );

            //result.RenderBindings.AddRange(
            //        databaseInfo.ForeignKey
            //        .Select(foreignKey => new TemplateBinding<ForeignKeyInfo>(foreignKey, this.SelectByReferencedPKTempate))
            //    );

            //List<(TableInfo tableInfoData, TableInfo tableInfoHistory, List<(ColumnInfo columnData, ColumnInfo columnHistory)> columnPairs)>?
            result.AddRenderBindings(
                    "UpdateTempate",
                    tablesUpdatePaired
                    .Select(t => new DataTemplateBinding<TableDataHistory>(
                            t,
                            t => t.TableData,
                            this.UpdateTempate)));

            result.AddRenderBindings(
                    "DeletePKTempate",
                    tablesUpdatePaired
                        .Select(t => new DataTemplateBinding<TableDataHistory>(
                            t,
                            t => t.TableData,
                            this.DeletePKTempate)));


            result.ReplacementBindings.AddRange(
                ConfigurationBound.CreateReplacementBinding<TableInfo>(this.ReplacementTableTemplates, databaseInfo.Tables)
                );
            //
            return result;
        }

        private static bool IsADataTable(TableInfo t) {
            return !(IsActivityTable(t) || IsAHistoryTable(t));
        }
        private static bool IsAHistoryTable(TableInfo t) {
            return string.Equals(t.Schema, "history", System.StringComparison.Ordinal);
        }

        private static bool IsActivityTable(TableInfo t) {
            return string.Equals(t.GetNameQ(), "[dbo].[Activity]", System.StringComparison.Ordinal);
        }
    }
}
