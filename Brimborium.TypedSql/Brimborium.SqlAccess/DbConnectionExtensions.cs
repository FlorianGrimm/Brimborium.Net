namespace Brimborium.SqlAccess;

public static class DbConnectionExtensions {
    public static IDisposable Connected(this IDbConnection dbConnection) {
        var wasClosed = dbConnection.State == ConnectionState.Closed;
        if (wasClosed) {
            dbConnection.Open();
        }

        return new ConnectedLock(dbConnection, wasClosed);
    }

    private sealed class ConnectedLock : IDisposable {
        private readonly IDbConnection m_DbConnection;
        private readonly bool m_WasClosed;

        public ConnectedLock(IDbConnection dbConnection, bool wasClosed) {
            this.m_DbConnection = dbConnection;
            this.m_WasClosed = wasClosed;
        }

        public void Dispose() {
            if (this.m_WasClosed) {
                this.m_DbConnection.Close();
            }
        }
    }
}