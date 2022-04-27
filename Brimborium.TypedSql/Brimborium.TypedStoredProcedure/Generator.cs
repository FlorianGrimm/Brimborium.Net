
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Brimborium.TypedStoredProcedure;
public partial class Generator {
    public static bool GenerateSqlAccessWrapper(
        IEnumerable<Type> types,
        string connectionString,
        string outputPath,
        DatabaseDefintion dbDef,
        PrintClass printClass,
        bool isForce) {

        var sbOutputImplementation = new StringBuilder();

        using var connection = new SqlConnection(connectionString);
        var serverConnection = new ServerConnection(connection);
        var server = new Server(serverConnection);
        var database = server.Databases[connection.Database];

        var ignoreTypePropertyNames = dbDef.GetIgnoreTypePropertyNames();
        var storedProcedureNames = dbDef.GetStoredProcedureNames();

        var regex = new Regex("^[0-9A-Za-z_]+$");
        var dctTypeNames = types.Where(t => regex.IsMatch(t.Name))
            .ToDictionary(t => t.Name, t => t.FullName, StringComparer.OrdinalIgnoreCase);

        var lstStoredProcedure = new List<StoredProcedure>();
        foreach (StoredProcedure? storedProcedure in database.StoredProcedures) {
            if (storedProcedure is object
                && storedProcedure.Schema is object
                ) {
                if (string.Equals(storedProcedure.Schema, "sys", StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }
                lstStoredProcedure.Add(storedProcedure);
            }
        }
        var lstAllStoredProcedure = new List<DatabaseStoredProcedure>();
        foreach (var sp in lstStoredProcedure.OrderBy(p => p.Schema).ThenBy(p => p.Name)) {
            var psp = new DatabaseStoredProcedure(
                sp,
                new StoredProcedureDefintion(sp.Schema, sp.Name, CSTypeDefinition.None, ExecutionMode.Unknown, CSTypeDefinition.None),
                new MemberDefinition[0],
                new StoredProcedureResultSet[0]);
            lstAllStoredProcedure.Add(psp);
        }
        var lstUsed = new List<PrintSPPair>();
        {
            var dctSPIsConfigured = lstAllStoredProcedure
                .Select(sp => sp.SPDefinition)
                .ToDictionary(sp => sp.SqlName, sp => false, System.StringComparer.OrdinalIgnoreCase);
            foreach (var spDef in dbDef.StoredProcedures) {
                if (dctSPIsConfigured.ContainsKey(spDef.SqlName)) {
                    dctSPIsConfigured[spDef.SqlName] = true;
                } else if (spDef.ExecutionMode == ExecutionMode.Ignore) {
                    // ignore
                } else {
                    var line = $"#warning {spDef.SqlName} is defined, cannot find in database.";
                    sbOutputImplementation.AppendLine(line);
                    System.Console.Error.WriteLine(line);
                }
            }
        }

        {
            var dctSPIsConfigured = dbDef.StoredProcedures.GroupBy(sp => sp.SqlName, sp => sp)
                .ToDictionary(sp => sp.Key, items => items.ToList(), System.StringComparer.OrdinalIgnoreCase);
            foreach (var dbSP in lstAllStoredProcedure) {
                if (dctSPIsConfigured.TryGetValue(dbSP.SPDefinition.SqlName, out var spDefs)) {
                    var resultSets = SQLUtility.DetermineResultSets(
                            connection,
                            dbSP,
                            (txt) => { sbOutputImplementation.AppendLine(txt); }
                        );
                    var dbSPParameters = dbSP with {
                        Parameters = ConvertParameter(dbSP.SP.Parameters),
                        ResultSets = resultSets.ToArray()
                    };

                    foreach (var spDef in spDefs) {
                        if (spDef.Enabled) {
                            var printSPPair = new PrintSPPair(dbSPParameters, spDef);
                            lstUsed.Add(printSPPair);
                        }
                    }
                } else {
                    var argsTypeName = dbSP.SPDefinition.Name.Replace("_", "") + "Args";
                    if (!dctTypeNames.TryGetValue(argsTypeName, out var argsTypeFullName)) {
                        argsTypeName = dbSP.SPDefinition.Name.Replace("_Get", "").Replace("_get", "").Replace("_Put", "").Replace("_put", "").Replace("_Del", "").Replace("_del", "") + "Args";
                        if (!dctTypeNames.TryGetValue(argsTypeName, out argsTypeFullName)) {
                            argsTypeName = dbSP.SPDefinition.Name.Replace("_Get", "").Replace("_get", "").Replace("_Put", "").Replace("_put", "").Replace("_Del", "").Replace("_del", "");
                            if (!dctTypeNames.TryGetValue(argsTypeName, out argsTypeFullName)) {
                            }
                        }
                    }
                    var returnTypeName = dbSP.SPDefinition.Name.Replace("_", "") + "Return";
                    if (!dctTypeNames.TryGetValue(returnTypeName, out var returnTypeFullName)) {
                        returnTypeName = dbSP.SPDefinition.Name.Replace("_Get", "").Replace("_get", "").Replace("_Put", "").Replace("_put", "").Replace("_Del", "").Replace("_del", "") + "Return";
                        if (!dctTypeNames.TryGetValue(returnTypeName, out returnTypeFullName)) {
                            returnTypeName = dbSP.SPDefinition.Name.Replace("_Get", "").Replace("_get", "").Replace("_Put", "").Replace("_put", "").Replace("_Del", "").Replace("_del", "");
                            if (!dctTypeNames.TryGetValue(returnTypeName, out returnTypeFullName)) {
                            }
                        }
                    }
                    var returnCSTypeDefinition = string.IsNullOrEmpty(returnTypeFullName)
                        ? "CSTypeDefinition.None"
                        : $"CSTypeDefinition.TypeOf<{returnTypeFullName}>(isList:true)"
                        ;
                    var argsCSTypeDefinition = string.IsNullOrEmpty(argsTypeFullName)
                        ? "CSTypeDefinition.None"
                        : $"CSTypeDefinition.TypeOf<{argsTypeFullName}>()"
                        ;
                    var line = $"#warning new StoredProcedureDefintion(\"{dbSP.SPDefinition.Schema}\", \"{dbSP.SPDefinition.Name}\", {argsCSTypeDefinition}, ExecutionMode.Unknown, {returnCSTypeDefinition}),";
                    sbOutputImplementation.AppendLine(line);
                    System.Console.Error.WriteLine(line);
                }
            }
        }

        {
            var success = sbOutputImplementation.Length == 0;
            var ctxt = new PrintContext(sbOutputImplementation);
            sbOutputImplementation.AppendLine("#if true");
            sbOutputImplementation.AppendLine("#nullable enable");
            sbOutputImplementation.AppendLine("");
            printFileImplementation(lstUsed, printClass, storedProcedureNames, ignoreTypePropertyNames, ctxt);
            sbOutputImplementation.AppendLine("");
            sbOutputImplementation.AppendLine("#endif");

            if (WriteText(outputPath, sbOutputImplementation.ToString())) {
                Console.WriteLine($"Modfied: {outputPath}");
            } else {
                Console.WriteLine($"not changed: {outputPath}");
            }
            return success;
        }
    }

    public static bool GenerateModel(
        //IEnumerable<Type> types,
        string connectionString,
        string outputPath,
        //DatabaseDefintion dbDef,
        PrintClass printClass) {

        var sbOutputImplementation = new StringBuilder();

        using var connection = new SqlConnection(connectionString);
        var serverConnection = new ServerConnection(connection);
        var server = new Server(serverConnection);
        var database = server.Databases[connection.Database];
        var lstTablePrimaryKey = database.Tables.Cast<Table>().OrderBy(table => table.Schema).ThenBy(table => table.Name).Select(table => {
            var primaryKey = table.Indexes.Cast<Microsoft.SqlServer.Management.Smo.Index>().FirstOrDefault(index => index.IndexKeyType == IndexKeyType.DriPrimaryKey);
            if (primaryKey is not null) {
                foreach (var indexedColumn in primaryKey.IndexedColumns.Cast<IndexedColumn>()) {

                }
            }
            return (table, primaryKey);
        }).Where(t => t.primaryKey != null).ToList();
        //printClass.ClassName

        // Solvin.OneTS.Model
        var ctxt = new PrintContext(sbOutputImplementation);
        sbOutputImplementation.AppendLine("#if true");
        /*
        sbOutputImplementation.AppendLine("using System;");
        */
        sbOutputImplementation.AppendLine("");

        sbOutputImplementation.Append("namespace ");
        sbOutputImplementation.Append(printClass.Namespace);
        sbOutputImplementation.AppendLine(" {");

        foreach (var t in lstTablePrimaryKey) {
            var (table, primaryKey) = t;
            if (string.Equals(table.Schema, "sys", StringComparison.OrdinalIgnoreCase)) {
                continue;
            }
            if (string.Equals(table.Name, "sysdiagrams", StringComparison.OrdinalIgnoreCase)) {
                continue;
            }
            sbOutputImplementation.AppendLine($"    public sealed partial record {table.Name}PK (");

            var indexedColumns = primaryKey!.IndexedColumns.Cast<IndexedColumn>().Select(c => {
                var cName = c.Name;
                var column = table.Columns.Cast<Column>().FirstOrDefault(column => string.Equals(column.Name, cName, StringComparison.InvariantCultureIgnoreCase));
                if (column is null) {
                    return string.Empty;
                } else {
                    var sqlDataType = column.DataType.SqlDataType;
                    string csType;
                    if (sqlDataType == SqlDataType.UniqueIdentifier) {
                        csType = "Guid";
                    } else if (sqlDataType == SqlDataType.Int) {
                        csType = "int";
                    } else if (sqlDataType == SqlDataType.BigInt) {
                        csType = "long";
                    } else if (sqlDataType == SqlDataType.Date) {
                        csType = "System.DateTime";
                    } else if (sqlDataType == SqlDataType.DateTime) {
                        csType = "System.DateTime";
                    } else if (sqlDataType == SqlDataType.DateTime2) {
                        csType = "System.DateTime";
                    } else if (sqlDataType == SqlDataType.VarChar) {
                        csType = "string";
                    } else if (sqlDataType == SqlDataType.VarCharMax) {
                        csType = "string";
                    } else if (sqlDataType == SqlDataType.NVarChar) {
                        csType = "string";
                    } else if (sqlDataType == SqlDataType.NVarCharMax) {
                        csType = "string";
                    } else if (sqlDataType == SqlDataType.DateTimeOffset) {
                        csType = "System.DateTimeOffset";
                    } else {
                        csType = "object";
                    }
                    return $"{csType} {cName}";
                }
            }).Where(line => !string.IsNullOrEmpty(line)).ToList();
            sbOutputImplementation.AppendLine("        " + string.Join(",\r\n        ", indexedColumns));
            sbOutputImplementation.AppendLine("        ) : IPrimaryKey;");
            sbOutputImplementation.AppendLine("");
        }

        sbOutputImplementation.AppendLine("}");
        sbOutputImplementation.AppendLine("");
        sbOutputImplementation.AppendLine("#endif");

        if (WriteText(outputPath, sbOutputImplementation.ToString())) {
            Console.WriteLine($"Modfied: {outputPath}");
            return true;
        } else {
            Console.WriteLine($"not changed: {outputPath}");
            return false;
        }
    }

    public static bool WriteText(string fileName, string fileContent) {
#if true
        if (System.IO.File.Exists(fileName)) {
            var oldContent = System.IO.File.ReadAllText(fileName);
            if (string.CompareOrdinal(oldContent, fileContent) == 0) {
                return false;
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
    private static MemberDefinition[] ConvertParameter(StoredProcedureParameterCollection parameters) {
        var result = new List<MemberDefinition>();
        foreach (StoredProcedureParameter? spParameter in parameters) {
            if (spParameter is object) {
                result.Add(
                        new MemberDefinition(
                            spParameter.Name,
                            DataTypeHelper.ConvertType(spParameter.DataType))
                    );
            }
        }
        return result.ToArray();
    }

    private static void printFileImplementation(
            List<PrintSPPair> lstUsed,
            PrintClass printClass,
            DictGetStoredProcedureNames storedProcedureNames,
            DictIgnoreTypePropertyNames ignoreTypePropertyNames,
            PrintContext ctxt
        ) {
        /*
        ctxt.AppendLine("using System;");
        ctxt.AppendLine("using System.Collections.Generic;");
        ctxt.AppendLine("using System.Data;");
        ctxt.AppendLine("using System.Threading.Tasks;");
        */
        ctxt.AppendLine("");

        printCurly($"namespace {printClass.Namespace}", "", ctxt, (ctxt) => {

            var lstInterfaceMethod = new List<string>();
            printCurly($"partial class {printClass.ClassName}", "", ctxt, (ctxt) => {
                var lstReaderDefinition = new List<ReaderDefinition>();
                var hsReaderDefinition = new HashSet<string>();
                foreach (var (dbSP, spDef) in lstUsed) {
                    if (dbSP is object && spDef is object) {
                        printExecuteMethod(
                            dbSP,
                            spDef,
                            hsReaderDefinition,
                            lstReaderDefinition,
                            lstInterfaceMethod,
                            storedProcedureNames,
                            ignoreTypePropertyNames,
                            ctxt);
                    }
                }

                foreach (var readerDef in lstReaderDefinition) {
                    printReader(readerDef, ignoreTypePropertyNames, ctxt);
                }
            });

            printCurly($"partial interface I{printClass.ClassName}", "", ctxt, (ctxt) => {
                foreach (var line in lstInterfaceMethod) {
                    ctxt.AppendLine(line);
                }
            });
        });
    }

    private static ReturnTypeNames getReturnType(CSTypeDefinition spDef_Return, ExecutionMode executionMode) {
        var isReturnVoid = spDef_Return.IsNone()
            || spDef_Return.IsVoid();
        var csReturnTypeRecord = isReturnVoid
            ? "void"
            : spDef_Return.Name ?? string.Empty;
        var csCompleteReturnType = csReturnTypeRecord;
        var csReturnTypeRecordQ = csReturnTypeRecord;
        if (spDef_Return.IsList) {
            csCompleteReturnType = $"List<{csCompleteReturnType}>";
        } else if (executionMode == ExecutionMode.QuerySingleOrDefault) {
            csCompleteReturnType = csReturnTypeRecordQ = $"{csReturnTypeRecordQ}?";
        }
        if (spDef_Return.IsAsync) {
            if (spDef_Return.Type == null) {
                csCompleteReturnType = "Task";
            } else {
                csCompleteReturnType = $"Task<{csCompleteReturnType}>";
            }
        }
        return new ReturnTypeNames(csCompleteReturnType, csReturnTypeRecord, csReturnTypeRecordQ);
    }

    private static void printExecuteMethod(
            DatabaseStoredProcedure dbSP,
            StoredProcedureDefintion spDef,
            HashSet<string> hsReaderDefinition,
            List<ReaderDefinition> lstReaderDefinition,
            List<string> lstInterfaceMethod,
            DictGetStoredProcedureNames storedProcedureNames,
            DictIgnoreTypePropertyNames ignoreTypePropertyNames,
            PrintContext ctxt
        ) {
        var spDef_Return = spDef.Return;
        var spDef_Argument = spDef.Argument;
        var spDef_ExecutionMode = spDef.ExecutionMode;

        var (csCompleteReturnType, csReturnTypeRecord, csReturnTypeRecordQ) = getReturnType(spDef_Return, spDef_ExecutionMode);

        var csAsyncModifier = "";
        //string methodName = $"Execute{dbSP.SP.Name}";
        if (storedProcedureNames.TryGetValue(dbSP.SPDefinition.SqlName, out var methodName)) {
        } else {
            methodName = $"Execute{dbSP.SP.Name}";
        }
        if (spDef_Return.IsAsync) {
            methodName = $"{methodName }Async";
            csAsyncModifier = "async ";
        }
        var nonArgs = spDef_Argument.IsNone()
            || spDef_Argument.IsVoid();
        var csArgs = spDef_Argument is null || nonArgs
            ? ""
            : $"{spDef_Argument.Name} args";
        //
        //var csMethod = $"{csCompleteReturnType} {methodName}({csArgs}IDbTransaction? tx = null) ";
        var csMethod = $"{csCompleteReturnType} {methodName}({csArgs})";
        lstInterfaceMethod.Add($"{csMethod};");
        printCurly($"public {csAsyncModifier}{csMethod} ", Environment.NewLine, ctxt, (ctxt) => {
            //
            printCurly($"using(var cmd = this.CreateCommand(\"{dbSP.SPDefinition.SqlName}\", CommandType.StoredProcedure))", "", ctxt, (ctxt) => {
                var argument_Type = spDef_Argument?.Type;
                if (spDef_Argument is object && argument_Type is object) {
                    #region Parameters
                    var dctProperties = argument_Type.GetProperties()
                        .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
                    foreach (var dbSPParameter in dbSP.Parameters) {
                        var parameterName = dbSPParameter.Name;
                        var nameNoAt = parameterName.TrimStart('@');
                        string propertyName;
                        if (dctProperties.TryGetValue(nameNoAt, out var property)) {
                            propertyName = property.Name;
                        } else {
                            ctxt.AppendLineAndError($"#warning StoredProcedure {dbSP.SPDefinition.SqlName} defines parameter {parameterName} no matching property found.");
                            continue;
                        }

                        MemberDefinition? spDef_Argument_Member = null;
                        if (spDef_Argument.Members is object) {
                            spDef_Argument_Member = spDef_Argument.Members.FirstOrDefault(m => string.Equals(m.Name, propertyName, StringComparison.OrdinalIgnoreCase));
                        }

                        var propertyType = property.PropertyType; //parameter.Type.Type;
                        var propertyTypeNotNull = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

                        var sqlDataType = dbSPParameter.Type.SqlDataType.GetValueOrDefault();
                        var maximumLength = dbSPParameter.Type.MaximumLength.GetValueOrDefault();
                        if (sqlDataType == SqlDataType.UserDefinedTableType) {
                            //
                        } else {
                            if (dbSPParameter.Type.Type == null) {
                                throw new ArgumentException($"SP:{dbSP.SP.Name}; Parameter:{parameterName};");
                            }
                            if (propertyTypeNotNull != dbSPParameter.Type.Type) {
                                if (propertyTypeNotNull.IsEnum
                                    && (dbSPParameter.Type.Type == typeof(int) || dbSPParameter.Type.Type == typeof(short))
                                    ) {
                                    // OK 
                                } else {
                                    ctxt.AppendLineAndError($"#warning StoredProcedure {dbSP.SPDefinition.SqlName} PropertyType parameter.Type.{property.PropertyType.FullName} {dbSPParameter.Type.Type.FullName}");
                                }
                            }
                        }

                        if (propertyTypeNotNull.IsEnum) {
                            if (ReferenceEquals(propertyType, propertyTypeNotNull)) {
                                ctxt.AppendLine($"this.AddParameterInt(cmd, \"{parameterName}\", (int)args.{propertyName});");
                            } else {
                                ctxt.AppendLine($"this.AddParameterInt(cmd, \"{parameterName}\", (int?)args.{propertyName});");
                            }
                        } else {
                            switch (sqlDataType) {
                                case SqlDataType.None:
                                    ctxt.AppendLineAndError($"#warning NYI sqlDataType ${sqlDataType}");
                                    break;
                                case SqlDataType.BigInt:
                                    ctxt.AppendLine($"this.AddParameterLong(cmd, \"{parameterName}\", args.{propertyName});");
                                    break;
                                case SqlDataType.Binary:
                                    ctxt.AppendLine($"this.AddParameterBinary(cmd, \"{parameterName}\", args.{propertyName});");
                                    break;
                                case SqlDataType.Bit:
                                    ctxt.AppendLine($"this.AddParameterBoolean(cmd, \"{parameterName}\", args.{propertyName});");
                                    break;
                                case SqlDataType.Char:
                                    ctxt.AppendLineAndError($"#warning NYI sqlDataType ${sqlDataType}");
                                    break;
                                case SqlDataType.DateTime:
                                    ctxt.AppendLine($"this.AddParameterDateTime(cmd, \"{parameterName}\", SqlDbType.DateTime, args.{propertyName});");
                                    break;
                                case SqlDataType.Decimal:
                                    ctxt.AppendLine($"this.AddParameterDecimal(cmd, \"{parameterName}\", SqlDbType.Decimal, args.{propertyName});");
                                    break;
                                case SqlDataType.Float:
                                    ctxt.AppendLine($"this.AddParameterDouble(cmd, \"{parameterName}\", SqlDbType.Float, args.{propertyName});");
                                    break;
                                case SqlDataType.Image:
                                    ctxt.AppendLine($"this.AddParameterImage(cmd, \"{parameterName}\", args.{propertyName});");
                                    break;
                                case SqlDataType.Int:
                                    ctxt.AppendLine($"this.AddParameterInt(cmd, \"{parameterName}\", args.{propertyName});");
                                    break;
                                case SqlDataType.Money:
                                    ctxt.AppendLine($"this.AddParameterDecimal(cmd, \"{parameterName}\", SqlDbType.Money, args.{propertyName});");
                                    break;
                                case SqlDataType.NChar:
                                    ctxt.AppendLine($"this.AddParameterString(cmd, \"{parameterName}\", SqlDbType.NChar, {maximumLength}, args.{propertyName});");
                                    break;
                                case SqlDataType.NText:
                                    ctxt.AppendLine($"this.AddParameterString(cmd, \"{parameterName}\", SqlDbType.NText, -1, args.{propertyName});");
                                    break;
                                case SqlDataType.NVarChar:
                                    ctxt.AppendLine($"this.AddParameterString(cmd, \"{parameterName}\", SqlDbType.NVarChar, {maximumLength}, args.{propertyName});");
                                    break;
                                case SqlDataType.NVarCharMax:
                                    ctxt.AppendLine($"this.AddParameterString(cmd, \"{parameterName}\", SqlDbType.NVarChar, -1, args.{propertyName});");
                                    break;
                                case SqlDataType.Real:
                                    ctxt.AppendLine($"this.AddParameterFloat(cmd, \"{parameterName}\", SqlDbType.Real, args.{propertyName});");
                                    break;
                                case SqlDataType.SmallDateTime:
                                    ctxt.Output.AppendLine($"#warning NYI sqlDataType ${sqlDataType}");
                                    break;
                                case SqlDataType.SmallInt:
                                    ctxt.AppendLine($"this.AddParameterShort(cmd, \"{parameterName}\", args.{propertyName}); /* {propertyType?.FullName} {propertyType?.IsEnum}*/");
                                    break;
                                case SqlDataType.SmallMoney:
                                    ctxt.Output.AppendLine($"#warning NYI sqlDataType ${sqlDataType}");
                                    break;
                                case SqlDataType.Text:
                                    ctxt.Output.AppendLine($"#warning NYI sqlDataType ${sqlDataType}");
                                    break;
                                case SqlDataType.Timestamp:
                                    ctxt.Output.AppendLine($"#warning NYI sqlDataType ${sqlDataType}");
                                    break;
                                case SqlDataType.TinyInt:
                                    ctxt.AppendLine($"this.AddParameterByte(cmd, \"{parameterName}\", args.{propertyName});");
                                    break;
                                case SqlDataType.UniqueIdentifier:
                                    ctxt.AppendLine($"this.AddParameterGuid(cmd, \"{dbSPParameter.Name}\", args.{propertyName});");
                                    break;
                                case SqlDataType.UserDefinedDataType:
                                    ctxt.Output.AppendLine($"#warning NYI sqlDataType ${sqlDataType}");
                                    break;
                                case SqlDataType.UserDefinedType:
                                    ctxt.Output.AppendLine($"#warning NYI sqlDataType ${sqlDataType}");
                                    break;
                                case SqlDataType.VarBinary:
                                    ctxt.Output.AppendLine($"#warning NYI sqlDataType ${sqlDataType}");
                                    break;
                                case SqlDataType.VarBinaryMax:
                                    ctxt.AppendLine($"this.AddParameterVarBinaryMax(cmd, \"{parameterName}\",  args.{propertyName});");
                                    break;
                                case SqlDataType.VarChar:
                                    ctxt.AppendLine($"this.AddParameterString(cmd, \"{parameterName}\", SqlDbType.VarChar, {maximumLength}, args.{propertyName});");
                                    break;
                                case SqlDataType.VarCharMax:
                                    ctxt.AppendLine($"this.AddParameterString(cmd, \"{parameterName}\", SqlDbType.NVarChar, -1, args.{propertyName});");
                                    break;
                                case SqlDataType.Variant:
                                    ctxt.Output.AppendLine($"#warning NYI sqlDataType ${sqlDataType}");
                                    break;
                                case SqlDataType.Xml:
                                    ctxt.Output.AppendLine($"#warning NYI sqlDataType ${sqlDataType}");
                                    break;
                                case SqlDataType.SysName:
                                    ctxt.AppendLine($"this.AddParameterString(cmd, \"{parameterName}\", SqlDbType.NVarChar, 512, args.{propertyName});");
                                    break;
                                case SqlDataType.Numeric:
                                    ctxt.AppendLine($"this.AddParameterDecimal(cmd, \"{parameterName}\", SqlDbType.Decimal, args.{propertyName});");
                                    break;
                                case SqlDataType.Date:
                                    ctxt.AppendLine($"this.AddParameterDateTime(cmd, \"{parameterName}\", SqlDbType.Date, {maximumLength}, args.{propertyName});");
                                    break;
                                case SqlDataType.Time:
                                    ctxt.AppendLine($"this.AddParameterDateTime(cmd, \"{parameterName}\", SqlDbType.Time, {maximumLength}, args.{propertyName});");
                                    break;
                                case SqlDataType.DateTimeOffset:
                                    ctxt.AppendLine($"this.AddParameterDateTimeOffset(cmd, \"{parameterName}\", args.{propertyName});");
                                    break;
                                case SqlDataType.DateTime2:
                                    ctxt.AppendLine($"this.AddParameterDateTime(cmd, \"{parameterName}\", SqlDbType.DateTime2, {maximumLength}, args.{propertyName});");
                                    break;
                                case SqlDataType.UserDefinedTableType:
                                    if (spDef_Argument_Member is null || spDef_Argument_Member.Type.ParameterConverter is null) {
                                        ctxt.Output.AppendLine($"#error spDef({spDef.Name}).Argument({propertyName}).ParameterConverter is null");
                                        ctxt.AppendLineAndError($"#warning this.AddParameterStructured(cmd, \"{parameterName}\", args.{propertyName}, ParameterConverter.ConvertParameter);");
                                    } else {
                                        ctxt.AppendLine($"this.AddParameterStructured(cmd, \"{parameterName}\", args.{propertyName}, {spDef_Argument_Member.Type.ParameterConverter.CsType.FullName}.ConvertParameter);");
                                    }
                                    break;
                                case SqlDataType.HierarchyId:
                                    ctxt.AppendLineAndError($"#warning NYI sqlDataType ${sqlDataType}");
                                    break;
                                case SqlDataType.Geometry:
                                    ctxt.AppendLineAndError($"#warning NYI sqlDataType ${sqlDataType}");
                                    break;
                                case SqlDataType.Geography:
                                    ctxt.AppendLineAndError($"#warning NYI sqlDataType ${sqlDataType}"); ;
                                    break;
                                default:
                                    ctxt.AppendLineAndError($"#warning NYI sqlDataType ${sqlDataType}");
                                    break;
                            }
                        }
                    }
                    #endregion Parameters
                } else if (dbSP.Parameters.Length > 0) {
                    ctxt.AppendLineAndError($"#warning {dbSP.SPDefinition.SqlName} C#-Parameters is none but SQL-Parameters are defined.");
                }
                printCommandExecuteMethod(
                    spDef_ExecutionMode,
                    dbSP,
                    dbSP.ResultSets,
                    lstReaderDefinition,
                    spDef_Return,
                    csReturnTypeRecord,
                    ctxt
                    );
            });
        });
        foreach (var readerDefinition in lstReaderDefinition) {
            if (hsReaderDefinition.Add(readerDefinition.ReaderName)) {
                printReader(readerDefinition, ignoreTypePropertyNames, ctxt);
            }
        }
        lstReaderDefinition.Clear();
    }

    private static void printCommandExecuteMethod(
        ExecutionMode executionMode,
        DatabaseStoredProcedure dbSP,
        StoredProcedureResultSet[] dbSP_ResultSets,
        List<ReaderDefinition> lstReaderDefinition,
        CSTypeDefinition spDef_Return,
        string csReturnTypeRecord,
        PrintContext ctxt) {
        switch (executionMode) {
            case ExecutionMode.Unknown:
                ctxt.AppendLineAndError($"#warning ExecutionMode.Unknown {dbSP.SPDefinition.SqlName}.");
                ctxt.AppendLine("throw new NotImplementedException();");
                break;

            case ExecutionMode.ExecuteNonQuery:
                if (spDef_Return.Type == typeof(int)) {
                    if (spDef_Return.IsAsync) {
                        ctxt.AppendLine("return await this.CommandExecuteNonQueryAsync(cmd);");
                    } else {
                        ctxt.AppendLine("return this.CommandExecuteNonQuery(cmd);");
                    }
                } else {
                    if (spDef_Return.IsAsync) {
                        ctxt.AppendLine("await this.CommandExecuteNonQueryAsync(cmd);");
                    } else {
                        ctxt.AppendLine("this.CommandExecuteNonQuery(cmd);");
                    }
                    ctxt.AppendLine("return;");
                }
                break;

            case ExecutionMode.ExecuteScalar:
                if (spDef_Return.IsAsync) {
                    ctxt.AppendLine($"return await this.CommandExecuteScalarAsync<{csReturnTypeRecord}>(cmd);");
                } else {
                    ctxt.AppendLine($"return this.CommandExecuteScalar<{csReturnTypeRecord}>(cmd);");
                }
                break;

            case ExecutionMode.QuerySingleOrDefault: {
                    if (dbSP_ResultSets.Length != 1) {
                        ctxt.AppendLineAndError($"#warning {dbSP.SP.Name} #ResultSets {dbSP_ResultSets.Length} actual, expected one!");
                        foreach (var dbResultSet in dbSP.ResultSetsToStrings()) {
                            ctxt.AppendLineAndError($"#warning Database: {dbResultSet}!");
                        }
                        ctxt.AppendLine("throw new NotImplementedException();");
                    } else {
                        var readerName = $"ReadRecord{dbSP.SP.Name}";
                        var readerDefinition = new ReaderDefinition(readerName, dbSP_ResultSets[0], spDef_Return, csReturnTypeRecord);
                        lstReaderDefinition.Add(readerDefinition);

                        if (spDef_Return.IsAsync) {
                            ctxt.AppendLine($"return await this.CommandQuerySingleOrDefaultAsync<{csReturnTypeRecord}>(cmd, {readerName});");
                        } else {
                            ctxt.AppendLine($"return this.CommandQuerySingleOrDefault<{csReturnTypeRecord}>(cmd, {readerName});");
                        }
                    }
                }
                break;

            case ExecutionMode.QuerySingle: {
                    if (dbSP_ResultSets.Length != 1) {
                        ctxt.AppendLineAndError($"#warning {dbSP.SP.Name} #ResultSets {dbSP_ResultSets.Length} actual, expected one!");
                        foreach (var dbResultSet in dbSP.ResultSetsToStrings()) {
                            ctxt.AppendLineAndError($"#warning Database: {dbResultSet}!");
                        }
                        ctxt.AppendLine("throw new NotImplementedException();");
                    } else {
                        var readerName = $"ReadRecord{dbSP.SP.Name}";
                        var readerDefinition = new ReaderDefinition(readerName, dbSP_ResultSets[0], spDef_Return, csReturnTypeRecord);
                        lstReaderDefinition.Add(readerDefinition);

                        if (spDef_Return.IsAsync) {
                            ctxt.AppendLine($"return await this.CommandQuerySingleAsync<{csReturnTypeRecord}>(cmd, {readerName});");
                        } else {
                            ctxt.AppendLine($"return this.CommandQuerySingle<{csReturnTypeRecord}>(cmd, {readerName});");
                        }
                    }
                }
                break;

            case ExecutionMode.Query: {
                    if (dbSP_ResultSets.Length != 1) {
                        ctxt.AppendLineAndError($"#warning {dbSP.SP.Name} #ResultSets {dbSP_ResultSets.Length} actual, expected one!");
                        foreach (var dbResultSet in dbSP.ResultSetsToStrings()) {
                            ctxt.AppendLineAndError($"#warning Database: {dbResultSet}!");
                        }
                        ctxt.AppendLine("throw new NotImplementedException();");
                    } else {
                        var readerName = $"ReadRecord{dbSP.SP.Name}";
                        var readerDefinition = new ReaderDefinition(readerName, dbSP_ResultSets[0], spDef_Return, csReturnTypeRecord);
                        lstReaderDefinition.Add(readerDefinition);

                        if (spDef_Return.IsAsync) {
                            ctxt.AppendLine($"return await this.CommandQueryAsync<{csReturnTypeRecord}>(cmd, {readerName});");
                        } else {
                            ctxt.AppendLine($"return this.CommandQuery<{csReturnTypeRecord}>(cmd, {readerName});");
                        }
                    }
                }
                break;

            case ExecutionMode.QueryMultiple:

                if (spDef_Return.Members is null || spDef_Return.Members.Length == 0) {
                    ctxt.AppendLineAndError($"#warning No Members defined! {dbSP.SPDefinition.SqlName} - {dbSP.ResultSets.Length} found in database!");
                    foreach (var dbResultSet in dbSP.ResultSetsToStrings()) {
                        ctxt.AppendLineAndError($"#warning Database: {dbResultSet}!");
                    }
                    ctxt.AppendLine("throw new NotImplementedException(\"{dbSP.SPDefinition.SqlName}\");");

                } else if (dbSP_ResultSets.Length != spDef_Return.Members.Length) {
                    ctxt.AppendLineAndError($"#warning Different count of resultset({dbSP_ResultSets.Length}) and members({spDef_Return.Members.Length}) defined!");
                    foreach (var dbResultSet in dbSP.ResultSetsToStrings()) {
                        ctxt.AppendLineAndError($"#warning Database: {dbResultSet}!");
                    }
                    ctxt.AppendLine("throw new NotImplementedException(\"{dbSP.SPDefinition.SqlName}\");");

                } else {
                    {
                        for (var idx = 0; idx < dbSP.ResultSets.Length; idx++) {
                            var spDefMembers = spDef_Return.Members[idx];

                            if (spDefMembers.Type.IsList) {
                                ctxt.AppendLine($"System.Collections.Generic.List<{spDefMembers.Type.Name}> result_{spDefMembers.Name} = new System.Collections.Generic.List<{spDefMembers.Type.Name}>();");
                            } else {
                                ctxt.AppendLine($"{spDefMembers.Type.Name} result_{spDefMembers.Name} = default!;");
                            }
                        }
                    }
                    printCurly(
                        spDef_Return.IsAsync
                            ? "await this.CommandQueryMultipleAsync(cmd, async (idx, reader) =>"
                            : "this.CommandQueryMultiple(cmd, (idx, reader) =>", $", {spDef_Return.Members.Length});",
                        ctxt, (ctxt) => {
                            for (var idx = 0; idx < dbSP.ResultSets.Length; idx++) {
                                var dbSPResultSet = dbSP.ResultSets[idx];
                                var spDefMember = spDef_Return.Members[idx];
                                var executionModeResultSet = spDefMember.Type.IsList ? ExecutionMode.Query : ExecutionMode.QuerySingleOrDefault;

                                var (csMembeCompleteReturnTypeIdx, csMemberReturnTypeRecordIdx, csMembeReturnTypeRecordQIdx) = getReturnType(spDefMember.Type, executionModeResultSet);
                                var readerName = $"ReadRecord{dbSP.SP.Name}_{idx}";
                                var readerDefinition = new ReaderDefinition(readerName, dbSPResultSet, spDefMember.Type, csMemberReturnTypeRecordIdx);

                                printCurly($"if (idx == {idx})", "", ctxt, (ctxt) => {
                                    //ctxt.AppendLine($"// {dbSPResultSet.Columns[0].Name}");
                                    if (spDefMember.Type.IsList) {
                                        if (spDef_Return.IsAsync) {
                                            ctxt.AppendLine($"result_{spDefMember.Name} = await this.CommandReadQueryAsync<{spDefMember.Type.Name}>(reader, {readerName});");
                                        } else {
                                            ctxt.AppendLine($"result_{spDefMember.Name} = this.CommandReadQuery<{spDefMember.Type.Name}>(reader, {readerName});");
                                        }
                                        lstReaderDefinition.Add(readerDefinition);
                                    } else if (spDefMember.Type.IsScalar) {
                                        ctxt.AppendLine($"result_{spDefMember.Name} = this.CommandReadScalar<{spDefMember.Type.Name}>(reader);");
                                    } else {
                                        if (spDefMember.Type.CanBeEmpty) {
                                            if (spDef_Return.IsAsync) {
                                                ctxt.AppendLine($"result_{spDefMember.Name} = (await this.CommandReadQuerySingleOrDefaultAsync<{spDefMember.Type.Name}>(reader, {readerName}))!;");
                                            } else {
                                                ctxt.AppendLine($"result_{spDefMember.Name} = (this.CommandReadQuerySingleOrDefault<{spDefMember.Type.Name}>(reader, {readerName}))!;");
                                            }
                                        } else {
                                            if (spDef_Return.IsAsync) {
                                                ctxt.AppendLine($"result_{spDefMember.Name} = await this.CommandReadQuerySingleAsync<{spDefMember.Type.Name}>(reader, {readerName});");
                                            } else {
                                                ctxt.AppendLine($"result_{spDefMember.Name} = this.CommandReadQuerySingle<{spDefMember.Type.Name}>(reader, {readerName});");
                                            }
                                        }
                                        lstReaderDefinition.Add(readerDefinition);
                                    }
                                });
                            }
                        });

                    printMultiReader(spDef_Return.Members, spDef_Return, csReturnTypeRecord, ctxt);
                }
                break;

            default:
                ctxt.AppendLineAndError($"#warning Unknown ExecutionMode {dbSP.SPDefinition.SqlName}");
                ctxt.AppendLine("throw new NotImplementedException(\"{dbSP.SPDefinition.SqlName}\");");
                break;
        }
    }

    private static void printReader(ReaderDefinition args, DictIgnoreTypePropertyNames ignoreTypePropertyNames, PrintContext ctxt) {
        var recordtype = args.spDef_Return.Type ?? throw new InvalidOperationException("spDef_Return.Type is null");
        var csReturnTypeRecord = args.csReturnTypeRecord;

        var mappingResults = MappingHelper.calculateMapping(recordtype, args.ResultSet, ignoreTypePropertyNames);

        printCurly($"protected {csReturnTypeRecord} {args.ReaderName}(Microsoft.Data.SqlClient.SqlDataReader reader)", Environment.NewLine, ctxt, (ctxt) => {
            if (mappingResults.AllMappings.Any(m => m.Mode == -1)) {
                var columnNames = string.Join(", ", args.ResultSet.Columns.Select(c => c.Name));
                ctxt.AppendLineAndError($"#warning ColumnNames: {columnNames}");
            }
            var nonmappingResults = mappingResults.AllProperties.Values.Where(v => !v.Matched && !v.Ignore).ToList();
            if (nonmappingResults.Count > 0) {
                ctxt.AppendLineAndError($"#warning not loaded from SQL {csReturnTypeRecord}");
                ctxt.AppendLine($"/*");
                var ctxtIndented = ctxt.Indented();
                ctxtIndented.AppendLine($"new TypePropertyNames(");

                ctxtIndented.AppendLine($"    typeof({csReturnTypeRecord}),");
                ctxtIndented.AppendLine($"    new string[] {{");
                var prefixList = "  ";
                foreach (var c in nonmappingResults) {
                    ctxtIndented.AppendLine($"        {prefixList}nameof({csReturnTypeRecord}.{c.CsName})");
                    prefixList = ", ";
                }
                ctxtIndented.AppendLine($"}}),");
                ctxt.AppendLine($"*/");
            }
            {
                foreach (var mappingResult in mappingResults.AllMappings.Where(m => m.Mode == -1)) {
                    ctxt.AppendLineAndError($"#warning Column not mapped {mappingResult.Index}: {mappingResult.Identity} {mappingResult.Column.TypeName} ({mappingResult.Column.TypeSize})");
                }
            }
            {
                foreach (var mappingResult in mappingResults.AllMappings.Where(m => m.Mode > 0)) {
                    if (!SQLUtility.DoesTypesMatch(typeName: mappingResult.Column.TypeName, mappingResult.ValueType)) {
                        var rc = MappingHelper.getCSReadCall(mappingResult.Column, mappingResult.ValueType);
                        if (rc.returnType.NotNullableType != rc.valueType.NotNullableType) {
                            ctxt.AppendLineAndError($"#warning Column types not match {mappingResult.Column.Name} \"{mappingResult.Column.TypeName}\", typeof({mappingResult.ValueType.FullName})");
                            ctxt.AppendLineAndError($"#warning {rc.returnType.NotNullableType.FullName} != {rc.valueType.NotNullableType.FullName} ");
                        }
#if false
                            var valueType = mappingResult.ValueType;
                            var underlyingValueType = Nullable.GetUnderlyingType(valueType);
                            valueType = underlyingValueType ?? valueType;
                            var isNullable = (underlyingValueType is object);
                            var readerMethodDefinition = SQLUtility.GetReaderMethodDefinition(
                                SQLUtility.ConvertSqlDataType(mappingResult.Column.TypeName)
                                );
                            Type readReturnType = (readerMethodDefinition is object)
                                ? (isNullable)
                                    ? readerMethodDefinition.ReadReturnTypeQ
                                    : readerMethodDefinition.ReadReturnType
                                : typeof(object);
                            if (valueType != readReturnType) {
                                var typeConverter = SQLUtility.GetTypeConverter(readReturnType, valueType);
                                if (!string.IsNullOrEmpty(typeConverter)) {
                                    readReturnType = valueType;
                                }
                            }
                            if (valueType != readReturnType) {
                                ctxt.AppendLineAndError($"#warning Column types not match {mappingResult.Column.Name} \"{mappingResult.Column.TypeName}\", typeof({mappingResult.ValueType.FullName})");
                            }
#endif
                    }
                }
            }
            string csNew;
            if (mappingResults.CtorParameter.Length == 0) {
                csNew = $"var result = new {csReturnTypeRecord}()";
            } else {
                ctxt.AppendLine($"var result = new {csReturnTypeRecord}(");
                for (int idx = 0, len = mappingResults.CtorParameter.Length; idx < len; idx++) {
                    var cp = mappingResults.CtorParameter[idx];
                    var seperator = idx == len - 1 ? "" : ",";
                    ctxt.AppendLine($"    @{cp.CsName}: {cp.csReadCall}{seperator}");
                }
                csNew = ")";
            }
            if (mappingResults.WriteProperties.Length == 0) {
                ctxt.AppendLine($"{csNew};");
            } else {
                printCurly(csNew, ";", ctxt, (ctxt) => {
                    for (int idx = 0, len = mappingResults.WriteProperties.Length; idx < len; idx++) {
                        var cp = mappingResults.WriteProperties[idx];
                        var seperator = idx == len - 1 ? "" : ",";
                        ctxt.AppendLine($"{cp.CsName} = {cp.csReadCall}{seperator}");
                    }
                });
            }
            for (int idx = 0, len = mappingResults.ReadListProperties.Length; idx < len; idx++) {
                var cp = mappingResults.ReadListProperties[idx];
                var seperator = idx == len - 1 ? "" : ";";
                ctxt.AppendLine($"result.{cp.CsName}.AddRange({cp.csReadCall});");
            }
            ctxt.AppendLine("return result;");
        });
    }
    private static void printMultiReader(
            MemberDefinition[]? members,
            CSTypeDefinition spDef_Return,
            string csReturnTypeRecord,
            PrintContext ctxt
        ) {
        if (members is null) {
            return;
        } else {

            var recordtype = spDef_Return.Type ?? throw new InvalidOperationException("spDef_Return.Type is null");
            // var csReturnTypeRecord = args.csReturnTypeRecord;
            var (
                ctorParameters,
                readableProperties,
                writableProperties,
                allProperties
            ) = MappingHelper.GetPropertyInfos(recordtype);

            var lstCtorParameters = new List<(string, System.Reflection.ParameterInfo)>();
            var lstReadableProperties = new List<(string, System.Reflection.PropertyInfo)>();
            var lstWritableProperties = new List<(string, System.Reflection.PropertyInfo)>();
            foreach (var member in members) {
                if (ctorParameters.TryGetValue(member.Name, out var ctorParameter)) {
                    lstCtorParameters.Add((member.Name, ctorParameter));
                    // ctxt.AppendLine($"// ctor {member.Name}");
                    continue;
                }
                if (writableProperties.TryGetValue(member.Name, out var writableProperty)) {
                    lstWritableProperties.Add((member.Name, writableProperty));
                    // ctxt.AppendLine($"// set {member.Name}");
                    continue;
                }
                if (readableProperties.TryGetValue(member.Name, out var readableProperty)) {
                    lstReadableProperties.Add((member.Name, readableProperty));
                    // ctxt.AppendLine($"// get {member.Name}");
                    continue;
                }
                ctxt.AppendLineAndError($"#warning not found {member.Name}");

            }
            //var mappingResults = calculateMapping(recordtype, args.ResultSet);
            //for (int idx = 0, len = mappingResults.AllMappings.Length; idx < len; idx++) {
            //    var cp = mappingResults.AllMappings[idx];
            //    ctxt.AppendLine($"// {cp.CsName}");
            //}
#if false
                if (mappingResults.AllMappings.Any(m => m.Mode == -1)) {
                    var columnNames = string.Join(", ", args.ResultSet.Columns.Select(c => c.Name));
                    ctxt.AppendLine($"// ColumnNames: {columnNames}");
                }
                var nonmappingResults = mappingResults.allProperties.Values.Where(v => !v.Matched).ToList();
                if (nonmappingResults.Count > 0) {
                    foreach (var c in nonmappingResults) {
                        ctxt.AppendLineAndError($"#warning Property not mached: {c.CsName}");
                    }
                }
                {
                    foreach (var mappingResult in mappingResults.AllMappings.Where(m => m.Mode == -1)) {
                        ctxt.AppendLineAndError($"#warning not mapped {mappingResult.Index}: {mappingResult.Identity} {mappingResult.Column.TypeName} ({mappingResult.Column.TypeSize})");
                    }
                }
#endif
#if true
            string csNew;
            if (lstCtorParameters.Count == 0) {
                csNew = $"var result = new {csReturnTypeRecord}()";
            } else {
                ctxt.AppendLine($"var result = new {csReturnTypeRecord}(");
                for (int idx = 0, len = lstCtorParameters.Count; idx < len; idx++) {
                    var pi = lstCtorParameters[idx];
                    var seperator = idx == len - 1 ? "" : ",";
                    ctxt.AppendLine($"    {pi.Item2.Name}: result_{pi.Item1}{seperator}");
                }
                csNew = ")";
            }

            if (lstWritableProperties.Count == 0) {
                ctxt.AppendLine($"{csNew};");
            } else {
                printCurly(csNew, ";", ctxt, (ctxt) => {
                    for (int idx = 0, len = lstWritableProperties.Count; idx < len; idx++) {
                        var pi = lstWritableProperties[idx];
                        var seperator = idx == len - 1 ? "" : ",";
                        ctxt.AppendLine($"{pi.Item1} = result_{pi.Item1}{seperator}");
                    }
                });
            }

            for (int idx = 0, len = lstReadableProperties.Count; idx < len; idx++) {
                var cp = lstReadableProperties[idx];
                var seperator = idx == len - 1 ? "" : ";";
                ctxt.AppendLine($"result.{cp.Item1}.AddRange(result_{cp.Item1});");
            }
            ctxt.AppendLine("return result;");
#endif
            //ctxt.AppendLine("throw new NotImplementedException();");
        }
    }

    private static void printCurly(string line1, string line2, PrintContext ctxt, Action<PrintContext> inner) {
        if (string.IsNullOrEmpty(line1)) { ctxt.AppendLine($"{{"); } else { ctxt.AppendLine($"{line1} {{"); }
        inner(ctxt.Indented());
        if (string.IsNullOrEmpty(line2)) { ctxt.AppendLine($"}}"); } else { ctxt.AppendLine($"}} {line2}"); }
    }
}
