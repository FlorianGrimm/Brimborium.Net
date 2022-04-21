using Brimborium.SqlAccess;

namespace Brimborium.TestSample.Service;

public partial class SqlAccess : SqlAccessBase {
    public SqlAccess(string connectionString)
        : base(connectionString) {
    }
}

