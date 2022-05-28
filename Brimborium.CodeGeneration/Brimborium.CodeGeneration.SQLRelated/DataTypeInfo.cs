namespace Brimborium.CodeGeneration.SQLRelated {
    public sealed record DataTypeInfo(
        SqlDataType SqlDataType
        ) {
        public static DataTypeInfo Create(Microsoft.SqlServer.Management.Smo.DataType dataType) {
            return new DataTypeInfo(
                dataType.SqlDataType
                );
        }
    }
}
