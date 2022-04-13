
using Microsoft.SqlServer.Management.Smo;

using System;

namespace Brimborium.TypedStoredProcedure {
    public static class DataTypeHelper {
        public static CSTypeDefinition ConvertType(DataType dataType) {
            switch (dataType.SqlDataType) {
                case SqlDataType.None:
                    break;
                case SqlDataType.BigInt:
                    return CSTypeDefinition.TypeOf<long>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.Binary:
                    return CSTypeDefinition.TypeOf<byte[]>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.Bit:
                    return CSTypeDefinition.TypeOf<bool>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.Char:
                    return CSTypeDefinition.TypeOf<string>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.DateTime:
                    return CSTypeDefinition.TypeOf<DateTime>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.Decimal:
                    return CSTypeDefinition.TypeOf<decimal>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.Float:
                    return CSTypeDefinition.TypeOf<double>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.Image:
                    return CSTypeDefinition.TypeOf<byte[]>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.Int:
                    return CSTypeDefinition.TypeOf<int>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.Money:
                    return CSTypeDefinition.TypeOf<decimal>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.NChar:
                    return CSTypeDefinition.TypeOf<string>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.NText:
                    return CSTypeDefinition.TypeOf<string>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.NVarChar:
                    return CSTypeDefinition.TypeOf<string>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.NVarCharMax:
                    return CSTypeDefinition.TypeOf<string>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.Real:
                    return CSTypeDefinition.TypeOf<float>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.SmallDateTime:
                    return CSTypeDefinition.TypeOf<DateTime>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.SmallInt:
                    return CSTypeDefinition.TypeOf<short>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.SmallMoney:
                    return CSTypeDefinition.TypeOf<decimal>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.Text:
                    return CSTypeDefinition.TypeOf<string>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.Timestamp:
                    return CSTypeDefinition.TypeOf<byte[]>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.TinyInt:
                    return CSTypeDefinition.TypeOf<byte>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.UniqueIdentifier:
                    return CSTypeDefinition.TypeOf<Guid>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.UserDefinedDataType:
                    break;
                case SqlDataType.UserDefinedType:
                    break;
                case SqlDataType.VarBinary:
                    return CSTypeDefinition.TypeOf<byte[]>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.VarBinaryMax:
                    return CSTypeDefinition.TypeOf<byte[]>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.VarChar:
                    return CSTypeDefinition.TypeOf<string>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.VarCharMax:
                    return CSTypeDefinition.TypeOf<string>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.Variant:
                    break;
                case SqlDataType.Xml:
                    break;
                case SqlDataType.SysName:
                    return CSTypeDefinition.TypeOf<string>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.Numeric:
                    return CSTypeDefinition.TypeOf<double>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.Date:
                    return CSTypeDefinition.TypeOf<DateTime>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.Time:
                    return CSTypeDefinition.TypeOf<DateTime>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.DateTimeOffset:
                    return CSTypeDefinition.TypeOf<DateTimeOffset>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.DateTime2:
                    return CSTypeDefinition.TypeOf<DateTime>(dataType.SqlDataType, dataType.MaximumLength);
                case SqlDataType.UserDefinedTableType:
                    return new CSTypeDefinition(typeof(object), dataType.Name, dataType.SqlDataType, null);
                case SqlDataType.HierarchyId:
                    break;
                case SqlDataType.Geometry:
                    break;
                case SqlDataType.Geography:
                    break;
                default:
                    break;
            }

            throw new NotImplementedException(dataType.Name);
        }
    }
}
