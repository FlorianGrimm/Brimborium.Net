using Brimborium.SqlAccess;

using System.Data;

namespace Brimborium.TestSample.Service;

public partial class SqlAccess : SqlDataAccessBase {
    public SqlAccess(IDbConnection connection, IDbTransaction transaction)
        : base(connection, transaction) {

    }
    public SqlAccess(string connectionString)
        : base(connectionString) {
    }
}

