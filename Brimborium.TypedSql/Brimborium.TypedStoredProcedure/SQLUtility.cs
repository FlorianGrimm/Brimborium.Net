
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brimborium.TypedStoredProcedure {
    public partial class SQLUtility {
        private static Dictionary<string, int> _StringTypes = new Dictionary<string, int>(
            new KeyValuePair<string, int>[] {
                new KeyValuePair<string, int>("char", 2 ),
                new KeyValuePair<string, int>("varchar", 4 ),
                new KeyValuePair<string, int>("nchar", 6 ),
                new KeyValuePair<string, int>("nvarchar", 8 )
            },
            StringComparer.OrdinalIgnoreCase
        );

        private static Dictionary<string, HashSet<Type>>? _SQLTypeNameToType;

        public static string GetStringType(string typeName, int stringType) {
            switch (stringType) {
                case 2: return "char";
                case 3: return "char";
                case 4: return "varchar";
                case 5: return "varchar";
                case 6: return "nchar";
                case 7: return "nchar";
                case 8: return "nvarchar";
                case 9: return "nvarchar";
                default: return typeName;
            }
        }

        public static int IsStringType(string typeName, int typeSize) {
            if (_StringTypes.TryGetValue(typeName, out var result)) {
                if (typeSize >= 0) {
                    return result;
                } else {
                    return result + 1;
                }
            } else {
                return 0;
            }
        }

        public static void AddTypesMatch(string typeName, Type valueType) {
            var sqlTypeNameToType = GetSQLTypeNameToType();
            if (!sqlTypeNameToType.TryGetValue(typeName, out var hs)) {
                hs = new HashSet<Type>();
                sqlTypeNameToType.Add(typeName, hs);
            }
            hs.Add(valueType);
        }

        private static Dictionary<string, HashSet<Type>> GetSQLTypeNameToType() {
            if (_SQLTypeNameToType is null) {
                _SQLTypeNameToType = new Dictionary<string, HashSet<Type>>(StringComparer.OrdinalIgnoreCase);
                AddTypesMatch("bigint", typeof(long));
                AddTypesMatch("bit", typeof(bool));
                AddTypesMatch("datetime2", typeof(DateTime));
                AddTypesMatch("int", typeof(int));
                AddTypesMatch("nvarchar", typeof(string));
                AddTypesMatch("nchar", typeof(string));
                AddTypesMatch("smallint", typeof(short));
                AddTypesMatch("uniqueidentifier", typeof(Guid));
                AddTypesMatch("varbinary", typeof(byte[]));
                AddTypesMatch("varchar", typeof(string));
                AddTypesMatch("char", typeof(string));
            }
            return _SQLTypeNameToType;
        }

        public static Microsoft.SqlServer.Management.Smo.SqlDataType ConvertSqlDataType(string typeName) {
            if (Enum.TryParse<Microsoft.SqlServer.Management.Smo.SqlDataType>(typeName, true, out var result)) {
                return result;
            } else {
                return Microsoft.SqlServer.Management.Smo.SqlDataType.None;
            }
        }

        private static Dictionary<Microsoft.SqlServer.Management.Smo.SqlDataType, ReaderMethodDefinition> ReaderMethodDefinition = new Dictionary<Microsoft.SqlServer.Management.Smo.SqlDataType, ReaderMethodDefinition>();

        public static ReaderMethodDefinition? GetReaderMethodDefinition(Microsoft.SqlServer.Management.Smo.SqlDataType sqlDataType) {
            if (ReaderMethodDefinition.Count == 0) {
                FillReaderMethodDefinition();
            }
            if (ReaderMethodDefinition.TryGetValue(sqlDataType, out var result)) {
                return result;
            } else {
                return null;
            }
        }

        private static void FillReaderMethodDefinition() {
            var defs = new ReaderMethodDefinition[] {
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.Bit,
                        "ReadBoolean", typeof(bool),
                        "ReadBooleanQ", typeof(bool?)),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.Char,
                        "ReadString", typeof(string),
                        "ReadString", typeof(string)),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.VarChar,
                        "ReadString", typeof(string),
                        "ReadString", typeof(string)),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.VarCharMax,
                        "ReadString", typeof(string),
                        "ReadString", typeof(string)),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.NChar,
                        "ReadString", typeof(string),
                        "ReadString", typeof(string)),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.NVarChar,
                        "ReadString", typeof(string),
                        "ReadString", typeof(string)),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.NVarCharMax,
                        "ReadString", typeof(string),
                        "ReadString", typeof(string)),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.DateTime,
                        "ReadDateTime", typeof(DateTime),
                        "ReadDateTimeQ", typeof(DateTime)),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.DateTime2,
                        "ReadDateTime", typeof(DateTime),
                        "ReadDateTimeQ", typeof(DateTime?)),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.DateTimeOffset,
                        "ReadDateTimeOffset", typeof(DateTimeOffset),
                        "ReadDateTimeOffsetQ", typeof(DateTime?)),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.SmallInt,
                        "ReadInt16", typeof(short),
                        "ReadInt16Q", typeof(short?)),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.Int,
                        "ReadInt32", typeof(int),
                        "ReadInt32Q", typeof(int?)),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.BigInt,
                        "ReadInt64", typeof(long),
                        "ReadInt64Q", typeof(long?)),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.Real,
                        "ReadFloat", typeof(float),
                        "ReadFloatQ", typeof(float?)),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.Float,
                        "ReadDouble", typeof(double),
                        "ReadDoubleQ", typeof(double?)),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.UniqueIdentifier,
                        "ReadGuid", typeof(Guid),
                        "ReadGuidQ", typeof(Guid?)),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.VarBinary,
                        "ReadByteArray", typeof(byte[]),
                        "ReadByteArrayQ", typeof(byte[])),
                    new ReaderMethodDefinition(
                        Microsoft.SqlServer.Management.Smo.SqlDataType.VarBinaryMax,
                        "ReadByteArray", typeof(byte[]),
                        "ReadByteArrayQ", typeof(byte[])),
                    //
                };
            foreach (var def in defs) {
                ReaderMethodDefinition.Add(def.SqlDataType, def);
            }
        }

        public static List<TypeConverterDefinition> TypeConverter = new List<TypeConverterDefinition>();
        public static void AddDefaultTypeConverter() {
            AddTypeConverter(typeof(short), typeof(int), "(int)");
        }

        public static void AddTypeConverter(Type fromType, Type toType, Type converterType, string converterMethod) {
            AddTypeConverter(fromType, toType, $"{converterType.FullName}.{converterMethod}");
        }
        public static void AddTypeConverter(Type fromType, Type toType, string converter) {
            TypeConverter.Add(new TypeConverterDefinition(fromType, toType, converter));
        }
        public static string GetTypeConverter(Type fromType, Type toType) {
            var result = TypeConverter.FirstOrDefault(tc => tc.FromType == fromType && tc.ToType == toType);
            if (result is object) {
                return result.Converter;
            } else {
                return string.Empty;
            }
        }

        public static bool DoesTypesMatch(string typeName, Type valueType) {
            var sqlTypeNameToType = GetSQLTypeNameToType();
            valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;
            if (valueType.IsEnum) {
                valueType = typeof(int);
            }
            if (sqlTypeNameToType.TryGetValue(typeName, out var hs)) {
                return hs.Contains(valueType);
            }
            return false;
        }
        public static List<StoredProcedureResultSet> DetermineResultSets(
                SqlConnection connection,
                DatabaseStoredProcedure dbSP,
                Action<string> errorWriteLine
            ) {
            try {
                return DescribeStoredProcedureResultSets(connection, dbSP.SP, errorWriteLine);
            } catch (Exception error) {
                errorWriteLine(error.ToString());
            }
            var result = new List<StoredProcedureResultSet>();
            try {
                var resultSet = readDescribeFirstResultSet(connection, dbSP.SPDefinition.SqlName, "");
                result.Add(resultSet);
            } catch (SqlException error) {
                errorWriteLine(error.ToString());
            }
            return result;
        }

        public static List<StoredProcedureResultSet> DescribeStoredProcedureResultSets(
            SqlConnection connection,
            StoredProcedure sp,
            Action<string> errorWriteLine
            ) {
            var sbParameter = new StringBuilder();
            var lstSelect = DescribeStoredProcedureResultSetsPrepare(sp, sbParameter);
            var results = DescribeStoredProcedureResultSetsSimplify(connection, sp, errorWriteLine, sbParameter, lstSelect);
            return results;
        }

        public static FindSelectResult DescribeStoredProcedureResultSetsPrepare(StoredProcedure sp, StringBuilder sbParameter) {
            var spTextBody = sp.TextBody;
            foreach (StoredProcedureParameter? parameter in sp.Parameters) {
                if (parameter is null) {
                    continue;
                }
                sbParameter.Append("DECLARE ");
                sbParameter.Append(parameter.Name);
                sbParameter.Append(" ");
                sbParameter.Append(parameter.DataType.ToString());
                sbParameter.AppendLine(";");
            }
            var parameterLength = sbParameter.Length;
            var sbSqlCode = new StringBuilder();
            sbSqlCode.Append(sbParameter);
            sbSqlCode.Append(spTextBody);

            var spCode = sp.TextHeader + sp.TextBody;

            var lstParseResult = new List<ParseResult>();
            var parseResult = Parser.Parse(spCode);

            var declareVisitor = new DeclareVisitor();
            declareVisitor.Visit(parseResult.Script.Batches.First());
            foreach (var decl in declareVisitor.Result) {
                sbParameter.Append(decl.Sql);
                sbParameter.AppendLine(";");
            }

            var fs2Result = new FindSelectResult(new List<FindSelectResult>(), true, null);
            var fs2 = new FindSelectRecursiveVisitor(fs2Result);
            fs2.Visit(parseResult.Script.Batches.First());
            var lstSelect = fs2Result;
            while (lstSelect.HasOnlyOneChild()) {
                lstSelect = lstSelect.Children.Single();
            }
            return lstSelect;
        }

        public static List<StoredProcedureResultSet> DescribeStoredProcedureResultSetsSimplify(SqlConnection connection, StoredProcedure sp, Action<string> errorWriteLine, StringBuilder sbParameter, FindSelectResult lstSelect) {
            var results = new List<StoredProcedureResultSet>();
            if (lstSelect is object) {
                var name = $"[{sp.Schema}].[{sp.Name}]";
                var resultRec = readDescribeForSelectRec(name, connection, sbParameter, lstSelect, errorWriteLine);
                resultRec = simplyfyResultSets(name, resultRec, errorWriteLine);

                if (resultRec.ResultSet is object) {
                    results.Add(resultRec.ResultSet);
                } else {
                    results.AddRange(
                        resultRec.Children
                            .Where(c => c.ResultSet is object)
                            .Select(c => c.ResultSet!));
                }
            }
            return results;
        }

        public static StoredProcedureResultSetNested readDescribeForSelectRec(
                string name,
                SqlConnection connection,
                StringBuilder sbParameter,
                FindSelectResult lstSelect,
                Action<string> errorWriteLine
                ) {
            if (lstSelect.SelectStatement is object) {
                var res = readDescribeForSelect(connection, sbParameter, lstSelect.SelectStatement);
                return new StoredProcedureResultSetNested(new List<StoredProcedureResultSetNested>(), lstSelect.IsSequential, res);
            } else {
                var children = new List<StoredProcedureResultSetNested>();
                foreach (var child in lstSelect.Children) {
                    var subResult = readDescribeForSelectRec(name, connection, sbParameter, child, errorWriteLine);
                    children.Add(subResult);
                }
                return new StoredProcedureResultSetNested(children, lstSelect.IsSequential, null);
            }
        }

        public static StoredProcedureResultSetNested simplyfyResultSets(
                string name,
                StoredProcedureResultSetNested current,
                Action<string> errorWriteLine) {
            if (current.ResultSet is object) {
                return current;
            }
            current = simplyfyResultSetsSequence(name, current, errorWriteLine);
            if (current.ResultSet is not null) {
                return current;
            }
            if (current.Children.All(child => child.ResultSet is not null)) {
                return current;
            }
            errorWriteLine($"Too complex..{name}");
            return current;
        }

        public static StoredProcedureResultSetNested simplyfyResultSetsSequence(
                string name,
                StoredProcedureResultSetNested current,
                Action<string> errorWriteLine) {
            if (current.ResultSet is object) {
                return current;
            }
            if (current.Children.Count == 0) {
                return current;
            }
            var simplyfiedChildren = current.Children.Select(child => simplyfyResultSetsSequence(name, child, errorWriteLine))
                        .Where(child => child.ResultSet is not null || !current.IsSequential || child.Children.Count > 0)
                        .ToList();
            var nextChildren = new List<StoredProcedureResultSetNested>();
            if (simplyfiedChildren.Count == 0) {
                return new StoredProcedureResultSetNested(nextChildren, current.IsSequential, null);
            }
            if (simplyfiedChildren.Count == 1) {
                if (simplyfiedChildren[0].ResultSet is not null) {
                    return new StoredProcedureResultSetNested(nextChildren, current.IsSequential, simplyfiedChildren[0].ResultSet);
                }
                if (simplyfiedChildren[0].Children.Count == 1) {
                    nextChildren.Add(simplyfiedChildren[0].Children.Single());
                    return new StoredProcedureResultSetNested(nextChildren, current.IsSequential && simplyfiedChildren[0].IsSequential, simplyfiedChildren[0].ResultSet);
                }
            }

            if (current.IsSequential) {
                if (simplyfiedChildren.All(child => child.ResultSet is not null || child.IsSequential || child.Children.Count == 0)) {
                    // current IsSequential
                    //      child1 IsSequential
                    //          child2 IsSequential
                    if (simplyfiedChildren.All(child => child.ResultSet is not null || child.IsSequential)) {
                        foreach (var child1 in simplyfiedChildren) {
                            if (child1.ResultSet is not null) {
                                nextChildren.Add(child1);
                            } else if (child1.Children.All(child => child.ResultSet is not null || child.IsSequential || child.Children.Count == 0)) {
                                foreach (var child2 in child1.Children) {
                                    if (child2.ResultSet is not null) {
                                        nextChildren.Add(child2);
                                    } else if (child2.Children.Count > 0) {
                                        nextChildren.Add(child2);
                                    }
                                }
                            } else if (child1.Children.Count > 0) {
                                nextChildren.Add(child1);
                            }
                        }
                        current.Children.AddRange(simplyfiedChildren);
                    }
                    return new StoredProcedureResultSetNested(nextChildren, current.IsSequential, null);
                }
            }
            if (!current.IsSequential) {
                if (simplyfiedChildren.All(child => child.ResultSet is not null)) {
                    var child0 = simplyfiedChildren[0];
                    var resultSet = child0.ResultSet;
                    StoredProcedureResultSet prevResultSet;
                    for (var idx = 1; idx < simplyfiedChildren.Count && resultSet is object; idx++) {
                        var childIdx = simplyfiedChildren[idx];
                        prevResultSet = resultSet;
                        if (resultSet.TryMergeResultSet(childIdx.ResultSet!, errorWriteLine, out resultSet)) {
                            // OK
                        } else {
                            errorWriteLine($"Different ResultSet options: {name} / {childIdx}");
                            errorWriteLine(prevResultSet.ToString());
                            errorWriteLine(childIdx.ResultSet!.ToString());
                        }
                    }
                    return new StoredProcedureResultSetNested(nextChildren, false, resultSet);
                }

                // if !Seq
                //    true Seq
                //    false Seq
                if (simplyfiedChildren.All(child => child.IsSequential && child.ResultSet is null && child.Children.All(child2 => child2.ResultSet is not null))) {
                    var child0 = simplyfiedChildren[0];
                    var resultSets = child0.Children.Select(child0 => child0.ResultSet!).ToList();
                    var ok = true;
                    for (var idx = 1; idx < simplyfiedChildren.Count && ok; idx++) {
                        var childIdx = simplyfiedChildren[idx];
                        if (childIdx.Children.Count != resultSets.Count) {
                            errorWriteLine($"Different ResultSet options: {name} / {childIdx}");
                            ok = false;
                        } else {
                            for (var idxRS = 0; idxRS < resultSets.Count; idxRS++) {
                                var resultSet = resultSets[idxRS];
                                var resultSetIdx = childIdx.Children[idxRS].ResultSet!;
                                if (resultSet.TryMergeResultSet(resultSetIdx, errorWriteLine, out resultSet)) {
                                    // OK
                                    resultSets[idxRS] = resultSet;
                                } else {
                                    errorWriteLine($"Different ResultSet options: {name} / {childIdx}");
                                    errorWriteLine(resultSets[idxRS].ToString());
                                    errorWriteLine(resultSetIdx.ToString());
                                    ok = false;
                                }
                            }
                        }
                    }
                    if (ok) {
                        nextChildren.AddRange(
                            resultSets.Select(rs => new StoredProcedureResultSetNested(new List<StoredProcedureResultSetNested>(), true, rs))
                        );
                    }
                    return new StoredProcedureResultSetNested(nextChildren, false, null);
                }

                if (simplyfiedChildren.All(child => child.ResultSet is not null || !child.IsSequential || child.Children.Count == 0)) {
                    foreach (var child1 in simplyfiedChildren) {
                        if (child1.ResultSet is not null) {
                            nextChildren.Add(child1);
                        } else if (child1.Children.All(child => child.ResultSet is not null || child.IsSequential || child.Children.Count == 0)) {
                            foreach (var child2 in child1.Children) {
                                if (child2.ResultSet is not null) {
                                    nextChildren.Add(child2);
                                } else if (child2.Children.Count > 0) {
                                    nextChildren.Add(child2);
                                }
                            }
                        } else {
                            nextChildren.Add(child1);
                        }
                    }
                    return new StoredProcedureResultSetNested(nextChildren, current.IsSequential, null);
                }
            }
            return current;
        }
        public static StoredProcedureResultSetNested simplyfyResultSetsNext(
                StoredProcedureResultSetNested current,
                Action<string> errorWriteLine) {
            if (current.ResultSet is object) {
                return current;
            }
            return current;
        }
#if old
        public static StoredProcedureResultSetNested readDescribeForSelectRec(
                string name,
                Microsoft.Data.SqlClient.SqlConnection connection,
                StringBuilder sbParameter,
                FindSelectResult lstSelect,
                Action<string> errorWriteLine
                ) {
            if (lstSelect.SelectStatement is object) {
                var res = readDescribeForSelect(connection, sbParameter, lstSelect.SelectStatement);
                return new StoredProcedureResultSetNested(new List<StoredProcedureResultSetNested>(), lstSelect.IsSequential, res);
            } else {
                var children = new List<StoredProcedureResultSetNested>();
                foreach (var item in lstSelect.Children) {
                    if (item.SelectStatement is object) {
                        var res = readDescribeForSelect(connection, sbParameter, item.SelectStatement);
                        children.Add(new StoredProcedureResultSetNested(new List<StoredProcedureResultSetNested>(), true, res));
                    } else {
                        var subResults = readDescribeForSelectRec(name, connection, sbParameter, item, errorWriteLine);
                        children.Add(subResults);
                    }
                }
                if (!lstSelect.IsSequential) {
                    if (children.Count == 0) {
                        return new StoredProcedureResultSetNested(new List<StoredProcedureResultSetNested>(), true, null);
                    }
                    if (children.All(c => c.ResultSet is object)) {
                        var child0 = children[0];
                        var resultSet = child0.ResultSet;
                        StoredProcedureResultSet prevResultSet;
                        for (int idx = 1; (idx < children.Count) && (resultSet is object); idx++) {
                            var childIdx = children[idx];
                            prevResultSet = resultSet;
                            if (resultSet.TryMergeResultSet(childIdx.ResultSet!, errorWriteLine, out resultSet)) {
                                // OK
                            } else {
                                errorWriteLine($"Different ResultSet options: {name} / {childIdx}");
                                errorWriteLine(prevResultSet.ToString());
                                errorWriteLine(childIdx.ResultSet!.ToString());
                            }
                        }
                        return new StoredProcedureResultSetNested(new List<StoredProcedureResultSetNested>(), false, resultSet);
                    }
                    errorWriteLine("Too complex resultSets");
                    return new StoredProcedureResultSetNested(new List<StoredProcedureResultSetNested>(), true, null);
                } else {
                    return new StoredProcedureResultSetNested(children, true, null);
                }
            }
        }
#endif

        public static StoredProcedureResultSet readDescribeForSelect(SqlConnection connection, StringBuilder sbParameter, SqlSelectStatement selectStatement) {
            var sb = new StringBuilder();
            sb.Append(sbParameter);
            sb.AppendLine();
            sb.AppendLine(selectStatement.Sql);
            var res = readDescribeFirstResultSet(connection, sb.ToString(), "");
            return res;
        }

        public static StoredProcedureResultSet readDescribeFirstResultSet(SqlConnection connection, string tsql, string tparams) {
            var lstColumns = new List<StoredProcedureResultColumn>();
            tsql = tsql.Replace("'", "''");
            if (string.IsNullOrEmpty(tparams)) { tparams = "null"; }
            using (var cmd = connection.CreateCommand()) {
                cmd.CommandText = $"EXECUTE sp_describe_first_result_set N'{tsql}', {tparams}, 1;";
                cmd.CommandType = System.Data.CommandType.Text;
                using (var reader = cmd.ExecuteReader()) {
                    var fieldCount = 0;
                    var idx_is_hidden = 0;
                    var idx_column_ordinal = 0;
                    var idx_name = 0;
                    var idx_system_type_name = 0;
                    var idx_source_schema = 0;
                    var idx_source_table = 0;
                    var idx_source_column = 0;
                    var idx_is_nullable = 0;
                    var dctNames = new Dictionary<string, int>();
                    while (reader.Read()) {
                        if (fieldCount == 0) {
                            fieldCount = reader.FieldCount;
                            for (var idx = 0; idx < fieldCount; idx++) {
                                dctNames[reader.GetName(idx)] = idx;
                            }
                            idx_is_hidden = dctNames["is_hidden"];
                            idx_column_ordinal = dctNames["column_ordinal"];
                            idx_name = dctNames["name"];
                            idx_is_nullable = dctNames["is_nullable"];
                            idx_system_type_name = dctNames["system_type_name"];
                            idx_source_schema = dctNames["source_schema"];
                            idx_source_table = dctNames["source_table"];
                            idx_source_column = dctNames["source_column"];
                        }
                        var record = new object[fieldCount];
                        reader.GetValues(record);
                        var system_type_name = (string)record[idx_system_type_name];
                        var arrSystemTypeName = system_type_name.Split('(', ')');
                        var typeName = arrSystemTypeName[0];
                        var typeSize = 0;
                        if (arrSystemTypeName.Length > 1) {
                            int.TryParse(arrSystemTypeName[1], out typeSize);
                        }
                        if ((bool)record[idx_is_hidden]) {
                            //
                        } else {
                            lstColumns.Add(new StoredProcedureResultColumn(
                                    (int)record[idx_column_ordinal] - 1,
                                    (string)(DBNull.Value == record[idx_name] ? "" : record[idx_name]),
                                    typeName,
                                    typeSize,
                                    (bool)record[idx_is_nullable]
                                ));
                        }
                    }
                }
            }
            var resultSet = new StoredProcedureResultSet(0, lstColumns.ToArray());
            return resultSet;
        }

    }
}
