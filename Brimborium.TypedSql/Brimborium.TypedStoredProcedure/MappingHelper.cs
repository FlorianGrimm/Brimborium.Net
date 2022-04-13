
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Brimborium.TypedStoredProcedure {
    public static class MappingHelper {
        public static GetPropertyInfo GetPropertyInfos(Type recordtype) {
            var propertyInfos = recordtype.GetProperties();


            var ctorParameters = new Dictionary<string, System.Reflection.ParameterInfo>(StringComparer.InvariantCultureIgnoreCase);
            var readableProperties = new Dictionary<string, System.Reflection.PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);
            var writableProperties = new Dictionary<string, System.Reflection.PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);
            var allProperties = new Dictionary<string, CalculateMappingProperty>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var pi in propertyInfos) {
                var piName = pi.Name;

                //var propertyTypeNotNullable = System.Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                //if ((
                //    (   propertyTypeNotNullable.IsValueType
                //        || propertyTypeNotNullable == typeof(string)
                //    )
                //    && pi.PropertyType.FullName!.StartsWith("System.")
                //    )==false) {
                //    continue;
                //}

                if (piName is object) {
                    if (!allProperties.ContainsKey(piName)) {
                        allProperties[piName] = new CalculateMappingProperty(piName);
                    }
                    if (pi.CanRead) {
                        readableProperties[piName] = pi;
                        allProperties[piName].Readable = pi;
                    }
                    if (pi.CanWrite) {
                        writableProperties[piName] = pi;
                        allProperties[piName].Writeable = pi;
                    }
                }
            }
            var arrConstructors = recordtype.GetConstructors().Where(ctor => {
                var parameters = ctor.GetParameters();
                if (parameters.Length == 1) {
                    if (parameters[0].ParameterType == recordtype) {
                        // copy ctor
                        return false;
                    }
                }
                return parameters.All(p => {
                    if (p.Name is null) {
                        return false;
                    } else if (!readableProperties.TryGetValue(p.Name, out var pi)) {
                        return false;
                    } else {
                        return pi.PropertyType == p.ParameterType;
                    }
                });
            }).ToArray();
            var cntConstructorParameters = arrConstructors.Length == 0 ? -1 : arrConstructors.Select(ctor => ctor.GetParameters().Length).Max();
            var ctorToUse = arrConstructors.FirstOrDefault(ctor => ctor.GetParameters().Length == cntConstructorParameters);
            if (ctorToUse is object) {
                foreach (var pi in ctorToUse.GetParameters()) {
                    var piName = pi.Name;
                    if (piName is object) {
                        ctorParameters[piName] = pi;
                        if (!allProperties.ContainsKey(piName)) {
                            allProperties[piName] = new CalculateMappingProperty(piName);
                            allProperties[piName].CtorPI = pi;
                        }
                    }
                }
            }

            return new GetPropertyInfo(
                ctorParameters,
                readableProperties,
                writableProperties,
                allProperties);
        }

        public static CalculateMappingReturn calculateMapping(
            Type recordtype,
            StoredProcedureResultSet resultSet,
            DictIgnoreTypePropertyNames ignoreTypePropertyNames) {
            var (
                    ctorParameters,
                    readableProperties,
                    writableProperties,
                    allProperties
                ) = GetPropertyInfos(recordtype);

            var lstMapping = new List<CalculateMappingReturnItem>();
            var lstCtorParameter = new List<CalculateMappingReturnItem>();
            var lstWriteProperties = new List<CalculateMappingReturnItem>();
            var lstReadListProperties = new List<CalculateMappingReturnItem>();

            for (var idxColumn = 0; idxColumn < resultSet.Columns.Length; idxColumn++) {
                var column = resultSet.Columns[idxColumn];

                ctorParameters.TryGetValue(column.Name, out var ctorPI);
                writableProperties.TryGetValue(column.Name, out var writeable);
                readableProperties.TryGetValue(column.Name, out var readable);

                CalculateMappingReturnItem ri;
                if (ctorPI is object && ctorPI.Name is object) {
                    var valueType = ctorPI.ParameterType;
                    var csReadCall = getCSReadCall(column, valueType).csRead;
                    ri = new CalculateMappingReturnItem(1, column, ctorPI.Name, ctorPI, writeable, readable, valueType, csReadCall);
                    lstMapping.Add(ri);
                    lstCtorParameter.Add(ri);
                    if (allProperties.TryGetValue(ctorPI.Name, out var mappingProperty)) {
                        mappingProperty.Matched = true;
                    }
                } else if (writeable is object && writeable.Name is object) {
                    var valueType = writeable.PropertyType;
                    var csReadCall = getCSReadCall(column, valueType).csRead;
                    ri = new CalculateMappingReturnItem(2, column, writeable.Name, ctorPI, writeable, readable, valueType, csReadCall);
                    lstMapping.Add(ri);
                    lstWriteProperties.Add(ri);
                    if (allProperties.TryGetValue(writeable.Name, out var mappingProperty)) {
                        mappingProperty.Matched = true;
                    }
                } else if (readable is object && readable.Name is object && TypeHelper.IsListOf(readable.PropertyType, out _)) {
                    var valueType = readable.PropertyType;
                    var csReadCall = getCSReadCall(column, valueType).csRead;
                    ri = new CalculateMappingReturnItem(3, column, readable.Name, ctorPI, writeable, readable, valueType, csReadCall);
                    lstMapping.Add(ri);
                    lstReadListProperties.Add(ri);
                    if (allProperties.TryGetValue(readable.Name, out var mappingProperty)) {
                        mappingProperty.Matched = true;
                    }
                } else {
                    var mode = -1;
                    if (ignoreTypePropertyNames.TryGetValue(recordtype, out var tpn)) {
                        if (tpn.Contains(column.Name)) {
                            mode = -2;
                        }
                    }
                    ri = new CalculateMappingReturnItem(mode, column, column.Name, ctorPI, writeable, readable, typeof(object), "default");
                    lstMapping.Add(ri);
                }
                {
                    if (ignoreTypePropertyNames.TryGetValue(recordtype, out var tpn)) {
                        foreach (var p in allProperties) {
                            if (tpn.Contains(p.Value.CsName)) {
                                p.Value.Ignore = true;
                            }
                        }
                    }
                }
            }

            return new CalculateMappingReturn(
                allProperties,
                lstMapping.ToArray(),
                lstCtorParameter.ToArray(),
                lstWriteProperties.ToArray(),
                lstReadListProperties.ToArray());
        }

        public static (string csRead, NullableTypeInfo valueType, NullableTypeInfo returnType) getCSReadCall(StoredProcedureResultColumn column, Type valueType) {
            var tiValueType = new NullableTypeInfo(valueType);
            var underlyingValueType = Nullable.GetUnderlyingType(valueType);
            valueType = underlyingValueType ?? valueType;
            //propertyType.GetCustomAttribute<AllowNullAttribute>
            //https://stackoverflow.com/questions/58453972/how-to-use-net-reflection-to-check-for-nullable-reference-type
            //https://github.com/dotnet/roslyn/blob/main/docs/features/nullable-metadata.md
            //readMethod = "";
            //readMethodCast = "";
            var csCast = string.Empty;
            var sqlDataType = SQLUtility.ConvertSqlDataType(column.TypeName);
            var readerMethodDefinition = sqlDataType != SqlDataType.None
                ? SQLUtility.GetReaderMethodDefinition(sqlDataType)
                : default;

            var readMethod = readerMethodDefinition is object
                ? tiValueType.IsNullableType
                    ? readerMethodDefinition.ReaderMethodQ
                    : readerMethodDefinition.ReaderMethod
                : string.Empty;

            var tiReadReturnType = new NullableTypeInfo(readerMethodDefinition is object
                ? tiValueType.IsNullableType
                    ? readerMethodDefinition.ReadReturnTypeQ
                    : readerMethodDefinition.ReadReturnType
                : typeof(object)
                );

            if (readMethod == string.Empty) {
                if (tiValueType.IsNullableType) {
                    readMethod = $"/*fallback*/ ReadValue<{tiValueType.NotNullableType.FullName}>";
                } else {
                    readMethod = $"/*fallback*/ ReadValueQ<{tiValueType.NotNullableType.FullName}>";
                }
            }
            if (tiValueType.NotNullableType.IsEnum) {
                if (sqlDataType == SqlDataType.Int
                    || sqlDataType == SqlDataType.SmallInt) {
                    if (tiValueType.IsNullableType) {
                        csCast = $"{tiValueType.NotNullableType.FullName}?";
                        //readReturnType = typeof(Nullable<>).MakeGenericType(new[] { valueType });
                        //underlyingTypeReadReturnType = Nullable.GetUnderlyingType(readReturnType) ?? readReturnType;
                        tiReadReturnType = tiValueType;
                    } else {
                        csCast = $"{tiValueType.NotNullableType.FullName}";
                        tiReadReturnType = tiValueType;
                    }
                }
            }
            var result = $"this.{readMethod}(reader, {column.Index})";
            if (!(csCast == string.Empty)) {
                result = $"({csCast}) ({result})";
            }
            if (tiValueType.NotNullableType != tiReadReturnType.NotNullableType) {
                var typeConverter = SQLUtility.GetTypeConverter(tiReadReturnType.GivenType, tiValueType.GivenType);
                if (!string.IsNullOrEmpty(typeConverter)) {
                    result = $"{typeConverter}({result})";
                    tiReadReturnType = tiValueType;
                } else {
                    result = $"/* converter {tiReadReturnType.GivenType.FullName} {tiValueType.GivenType.FullName} */({result})";
                }
            }
            if (tiValueType.NotNullableType != tiReadReturnType.NotNullableType) {
                result = $"(/*fallback*/ {tiValueType.NotNullableType.FullName}) ({result})";
            }
            return (csRead: result, valueType: tiValueType, returnType: tiReadReturnType);
#if no
            if ((sqlDataType == SqlDataType.NVarChar)
                || (sqlDataType == SqlDataType.NChar)
                || (sqlDataType == SqlDataType.VarChar)
                || (sqlDataType == SqlDataType.Char)
                ) {
                if (typeof(string) == valueType) {
                    return $"this.ReadString(reader, {column.Index})";
                } else {
#warning  think of
                    return $"this.ReadString(reader, {column.Index})";
                }
            } 

            if (sqlDataType == SqlDataType.Int) {
                if (valueType.IsEnum) {
                    if (isNullable) {
                        csCast = $"({valueType.FullName}?) ";
                    } else {
                        csCast = $"({valueType.FullName}) ";
                    }
                }

                if (typeof(int) == valueType) {
                    if (isNullable) {
                        return $"{csCast}this.ReadIntQ(reader, {column.Index})";
                    } else {
                        return $"{csCast}this.ReadInt(reader, {column.Index})";
                    }
                }
            }

            //if (sqlDataType == SqlDataType.TinyInt) { }

            if (sqlDataType == SqlDataType.SmallInt) {
                if (typeof(short) == valueType) {
                    if (isNullable) {
                        return $"this.ReadShortQ(reader, {column.Index})";
                    } else {
                        return $"this.ReadShort(reader, {column.Index})";
                    }
                }
            }
            if (sqlDataType == SqlDataType.BigInt) {
                if (typeof(long) == valueType) {
                    if (isNullable) {
                        return $"this.ReadLongQ(reader, {column.Index})";
                    } else {
                        return $"this.ReadLong(reader, {column.Index})";
                    }
                }
            }
            if (sqlDataType == SqlDataType.Bit) {
                if (typeof(bool) == valueType) {
                    if (isNullable) {
                        return $"this.ReadBooleanQ(reader, {column.Index})";
                    } else {
                        return $"this.ReadBoolean(reader, {column.Index})";
                    }
                }
            }
            if (sqlDataType == SqlDataType.UniqueIdentifier) {
                if (typeof(Guid) == valueType) {
                    if (isNullable) {
                        return $"this.ReadGuidQ(reader, {column.Index})";
                    } else {
                        return $"this.ReadGuid(reader, {column.Index})";
                    }
                }
            }
            if ((sqlDataType == SqlDataType.DateTime)
                || (sqlDataType == SqlDataType.DateTime2)) {
                if (typeof(DateTime) == valueType) {
                    if (isNullable) {
                        return $"this.ReadDateTimeQ(reader, {column.Index})";
                    } else {
                        return $"this.ReadDateTime(reader, {column.Index})";
                    }
                }
            }

            if (sqlDataType == SqlDataType.VarBinary) {

                if (typeof(byte[]) == valueType) {
                    if (isNullable) {
                        return $"this.ReadByteArrayQ(reader, {column.Index})";
                    } else {
                        return $"this.ReadByteArray(reader, {column.Index})";
                    }
                }
            }

            if (sqlDataType == SqlDataType.BigInt) {
            }

            if (isNullable) {
                if (isNullable) {
                    return $"this.ReadValueQ<{valueType.FullName}>(reader, {column.Index})";
                } else {
                    return $"this.ReadValue<{valueType.FullName}>(reader, {column.Index})";
                }
            }

            return string.Empty;
#endif
        }
    }
}
