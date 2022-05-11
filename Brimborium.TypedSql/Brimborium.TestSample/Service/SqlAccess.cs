namespace Brimborium.TestSample.Service;

[ExcludeFromCodeCoverage]
public partial class SqlAccess : SqlDataAccessBase {
    public SqlAccess(SqlConnection connection, IDbTransaction transaction)
        : base(connection, transaction) {
    }
}

