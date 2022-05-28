namespace Brimborium.CodeGeneration.SQLRelated {
    public static class DefaultTypeMappingHelper {
        public static string ConvertTypeSqlToCS(SqlDataType sqlDataType) {
            string csType;
            if (sqlDataType == SqlDataType.UniqueIdentifier) {
                csType = "Guid";
            } else if (sqlDataType == SqlDataType.Int) {
                csType = "int";
            } else if (sqlDataType == SqlDataType.BigInt) {
                csType = "long";
            } else if (sqlDataType == SqlDataType.Bit) {
                csType = "bool";
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
            } else if (sqlDataType == SqlDataType.SmallInt) {
                csType = "short";
            } else if (sqlDataType == SqlDataType.Timestamp) {
                csType = "byte[]";
            } else {
                csType = $"object /*{sqlDataType}*/";
            }
            return csType;
        }
    }
}