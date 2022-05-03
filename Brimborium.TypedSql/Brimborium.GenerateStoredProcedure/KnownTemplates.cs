using System;
using System.Collections.Generic;

namespace Brimborium.GenerateStoredProcedure {
    public class KnownTemplates {
        public readonly RenderTemplate<TableInfo> SelectTableColumns;
        public readonly RenderTemplate<List<ColumnInfo>> Columns;

        public readonly RenderTemplate<List<ColumnInfo>> ColumnsAsParameter;
        public readonly RenderTemplate<TableInfo> TableColumnsAsParameter;

        public readonly RenderTemplate<TableInfo> ColumnRowversion;

        public readonly RenderTemplate<(
            List<ColumnInfo> columns,
            ColumnInfo? columnRowVersion
            )> ColumnsAsParameterWithRowVersion;

        public readonly RenderTemplate<List<ColumnInfo>> ColumnsAsDeclareParameter;

        public readonly RenderTemplate<(
            List<ColumnInfo> columns,
            ColumnInfo? columnRowVersion,
            string prefix
            )> ColumnsAsDeclareParameterWithRowVersion;

        public KnownTemplates() {
            this.Columns = new RenderTemplate<List<ColumnInfo>>(
                Render: (columns, ctxt) => {
                    ctxt.AppendList(
                        columns,
                        (column, ctxt) => {
                            ctxt.AppendPartsLine(
                                column.GetNameQ(), ","
                                );
                        });
                }
                );

            this.ColumnRowversion = new RenderTemplate<TableInfo>(
                NameFn: t => $"ColumnRowversion.{t.GetNameQ()}",
                Render: (data, ctxt) => {
                    ctxt.AppendLine(
                        $"{data.ColumnRowversion.GetNameQ()} = CAST({data.ColumnRowversion.GetNameQ()} as BIGINT)");
                }
            );

            this.SelectTableColumns = new RenderTemplate<TableInfo>(
                NameFn: t => $"SelectTableColumns.{t.GetNameQ()}",
                Render: (data, ctxt) => {
                    ctxt.AppendList(
                        data.ColumnsWithRowversion,
                        (column, ctxt) => {
                            ctxt.AppendPartsLine(
                                column.GetReadNamedQ(),
                                ctxt.IfNotLast(",")
                                );
                        }
                        );
                    /*
                    ctxt.AppendList(
                        data.Columns,
                        (column, ctxt) => {
                            ctxt.AppendPartsLine(
                                column.GetNameQ(), ","
                                );
                        });
                    ctxt.AppendLine(
                        $"{data.ColumnRowversion.GetNameQ()} = CAST({data.ColumnRowversion.GetNameQ()} as BIGINT)");
                    */
                }
                );


            this.TableColumnsAsParameter = new RenderTemplate<TableInfo>(
                NameFn: t => $"TableColumnsAsParameter.{t.GetNameQ()}",
                Render: (tableInfo, ctxt) => {
                    var columns = tableInfo.Columns;
                    ctxt.AppendList(columns, (column, ctxt) => {
                        ctxt.AppendPartsLine(
                            column.GetNamePrefixed("@"),
                            " ",
                            column.GetParameterSqlDataType(),
                            ctxt.IfNotLast(",")
                            );
                    });
                }
            );
            this.ColumnsAsParameter = new RenderTemplate<List<ColumnInfo>>(
                Render: (columns, ctxt) => {
                    ctxt.AppendList(columns, (column, ctxt) => {
                        ctxt.AppendPartsLine(
                            column.GetNamePrefixed("@"),
                            " ",
                            column.GetParameterSqlDataType(),
                            ctxt.IfNotLast(",")
                            );
                    });
                }
            );

            this.ColumnsAsParameterWithRowVersion = new RenderTemplate<(
                List<ColumnInfo> columns,
                ColumnInfo? columnRowVersion
                )>(
                Render: (data, ctxt) => {
                    ctxt.AppendList(data.columns, (column, ctxt) => {
                        ctxt.AppendPartsLine(
                            column.GetNamePrefixed("@"),
                            " ",
                            column.GetParameterSqlDataType(),
                            ((ctxt.IsLast && (data.columnRowVersion is null)) ? string.Empty : ",")
                            );

                    });
                    if (data.columnRowVersion is not null) {
                        ctxt.AppendPartsLine(
                            data.columnRowVersion.GetNamePrefixed("@"),
                                " BIGINT"
                                );
                    }
                }
            );

            this.ColumnsAsDeclareParameter = new RenderTemplate<List<ColumnInfo>>(
                Render: (columns, ctxt) => {
                    ctxt.AppendList(columns, (column, ctxt) => {
                        ctxt.AppendPartsLine(
                            "DECLARE ",
                            column.GetNamePrefixed("@"),
                            " ",
                            column.GetSqlDataType(),
                            ";"
                            );
                    });
                }
            );

            this.ColumnsAsDeclareParameterWithRowVersion = new RenderTemplate<(
                List<ColumnInfo> columns,
                ColumnInfo? columnRowVersion,
                string prefix
                )>(
                Render: (data, ctxt) => {
                    ctxt.AppendList(data.columns, (column, ctxt) => {
                        ctxt.AppendPartsLine(
                            "DECLARE ",
                            column.GetNamePrefixed(data.prefix),
                            " ",
                            column.GetSqlDataType(),
                            ";"
                            );
                    });
                    if (data.columnRowVersion is not null) {
                        ctxt.AppendPartsLine(
                            "DECLARE ",
                            data.columnRowVersion.GetNamePrefixed(data.prefix),
                            " BIGINT;"
                            );
                    }
                }
            );
        }

        public static void SqlCreateProcedure<T>(
            T data,
            PrintContext ctxt,
            string schema,
            string name,
            Action<T, PrintContext> parameter,
            Action<T, PrintContext> bodyBlock
            ) {
            var ctxtIndented = ctxt.GetIndented();
            ctxt.AppendLine($"CREATE PROCEDURE [{schema}].[{name}]");
            parameter(data, ctxtIndented);
            ctxt.AppendLine("AS BEGIN");
            ctxtIndented.AppendLine("SET NOCOUNT ON;");
            ctxtIndented.AppendLine();
            bodyBlock(data, ctxtIndented);
            ctxt.AppendLine("END;");
        }

        public static void SqlSelect<T>(
            T data,
            PrintContext ctxt,
            long? top,
            Action<T, PrintContext> columnsBlock,
            Action<T, PrintContext> fromBlock,
            Action<T, PrintContext>? whereBlock = default,
            Action<T, PrintContext>? groupByBlock = default,
            Action<T, PrintContext>? havingBlock = default,
            Action<T, PrintContext>? orderByBlock = default,
            bool appendSemicolon = true
        ) {
            var ctxtIndented = ctxt.GetIndented();
            var ctxtIndented2 = ctxtIndented.GetIndented();
            if (top.HasValue) {
                ctxt.AppendLine($"SELECT TOP({top})");
            } else {
                ctxt.AppendLine("SELECT");
            }
            columnsBlock(data, ctxtIndented2);

            ctxtIndented.AppendLine("FROM");
            fromBlock(data, ctxtIndented2);

            if (whereBlock is not null) {
                ctxtIndented.AppendLine("WHERE");
                whereBlock(data, ctxtIndented2);
            }
            if (groupByBlock is not null) {
                ctxtIndented.AppendLine("GROUP BY");
                groupByBlock(data, ctxtIndented2);
            }
            if (havingBlock is not null) {
                ctxtIndented.AppendLine("HAVING");
                havingBlock(data, ctxtIndented2);
            }
            if (orderByBlock is not null) {
                ctxtIndented.AppendLine("ORDER BY");
                orderByBlock(data, ctxtIndented2);
            }
            if (appendSemicolon) {
                ctxtIndented.AppendLine(";");
            }
        }

#if false

    KnownTemplates.SqlIf(
        data,
        ctxt,
        condition: (data, ctxt) => { },
        thenBlock: (data, ctxt) => { }
        );

#endif
        public static void SqlIf<T>(
            T data,
            PrintContext ctxt,
            Action<T, PrintContext> condition,
            Action<T, PrintContext> thenBlock,
            Action<T, PrintContext>? elseBlock = default
            ) {
            var ctxtIndented = ctxt.GetIndented();
            ctxt.Append("IF (");
            condition(data, ctxtIndented);
            ctxt.AppendLine(") BEGIN");
            thenBlock(data, ctxtIndented);
            if (elseBlock is null) {
                ctxt.AppendLine("END;");
            } else {
                ctxt.AppendLine("END ELSE BEGIN");
                elseBlock(data, ctxtIndented);
                ctxt.AppendLine("END;");
            }
        }

        public static void SqlInsertValues<T>(
            T data,
            PrintContext ctxt,
            string target,
            Action<T, PrintContext> nameBlock,
            Action<T, PrintContext> valuesBlock,
            Action<T, PrintContext>? outputBlock = default,
            Action<T, PrintContext>? outputIntoBlock = default
            ) {
            var ctxtIndented = ctxt.GetIndented();
            ctxt.AppendLine($"INSERT INTO {target} (");
            nameBlock(data, ctxtIndented);
            ctxt.AppendLine(")");
            if (outputBlock is not null) {
                ctxt.AppendLine("OUTPUT");
                ctxt.AppendIndented(
                    data,
                    outputBlock);

                if (outputIntoBlock is not null) {
                    ctxt.Append("INTO ");
                    outputIntoBlock(data, ctxt);
                }
            }

            ctxt.AppendLine("VALUES (");
            valuesBlock(data, ctxtIndented);
            ctxt.AppendLine(");");
        }

        public static void SqlUpdate<T>(
            T data,
            PrintContext ctxt,
            long? top,
            string target,
            Action<T, PrintContext> setBlock,
            Action<T, PrintContext>? whereBlock = default
            ) {
            var ctxtIndented1 = ctxt.GetIndented();
            var ctxtIndented2 = ctxtIndented1.GetIndented();
            if (top.HasValue) {
                ctxt.AppendPartsLine("UPDATE TOP(", top.Value.ToString(), ") ", target);
            } else {
                ctxt.AppendPartsLine("UPDATE ", target);
            }
            ctxtIndented1.AppendLine("SET");
            setBlock(data, ctxtIndented2);
            if (whereBlock is not null) {
                ctxtIndented1.AppendLine("WHERE");
                whereBlock(data, ctxtIndented2);
            }
            ctxt.AppendLine(";");
        }

        public static string SqlIsNull(string expression) {
            return $"({expression} IS NULL)";
        }

        public static string SqlIsNotNull(string expression) {
            return $"({expression} IS NOT NULL)";
        }
    }
}
