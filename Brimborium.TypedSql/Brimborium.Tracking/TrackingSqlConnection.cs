namespace Brimborium.Tracking;

public class TrackingSqlConnectionOption {
    public string? ConnectionString { get; set; }
}

public class TrackingSqlTransConnection
    : TrackingTransConnection
    , IDisposable {

    public static async Task<(SqlConnection connection, IDbTransaction transaction)> OpenAsync(
        string connectionString,
        CancellationToken cancellationToken = default(CancellationToken)
        ) {        
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync(System.Data.IsolationLevel.Unspecified, cancellationToken);
        return (connection, transaction);
    }

    protected SqlConnection? _Connection;
    protected IDbTransaction? _Transaction;

    public TrackingSqlTransConnection(SqlConnection connection, IDbTransaction? transaction) {
        this._Connection = connection;
        this._Transaction = transaction;
    }

    public TrackingSqlTransConnection(string connectionString) {
        this._Connection = new SqlConnection(connectionString);
        this._Transaction = this._Connection.BeginTransaction();
        //System.GC.SuppressFinalize(this);
    }


    public override async Task CommitAsync() {
        if (this._Connection is null) {
            throw new System.InvalidOperationException("no connection");
        } else if (this._Transaction is null) {
            throw new System.InvalidOperationException("no transaction");
        } else if (this._Transaction is Microsoft.Data.SqlClient.SqlTransaction sqlTransaction) {
            await sqlTransaction.CommitAsync();
            await sqlTransaction.DisposeAsync();
            await this._Connection.CloseAsync();
            //this._Transaction = null;
            //this._Connection = null;
            this.Dispose();
        } else {
            this._Transaction.Commit();
            //this._Transaction.Dispose();
            //this._Connection.Close();
            //this._Transaction = null;
            //this._Connection = null;
            this.Dispose();
        }
    }


    protected override bool Dispose(bool disposing) {
        var result = base.Dispose(disposing);
        if (result) {
            using (var c = this._Connection) {
                using (var t = this._Transaction) {
                    this._Transaction = null;
                    this._Connection = null;
                }
            }
        }
        return result;
    }
}