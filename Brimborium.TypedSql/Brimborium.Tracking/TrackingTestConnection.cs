namespace Brimborium.Tracking;

#warning weichei
public class TrackingTestConnection : ITrackingConnection {
    private readonly Func<TrackingTestConnection, Task<TrackingTestTransaction>>? _FuncBeginTransaction;

    public TrackingTestConnection(
        Func<TrackingTestConnection, Task<TrackingTestTransaction>>? funcBeginTransaction
        ) {
        this._FuncBeginTransaction = funcBeginTransaction;
    }

    public async Task<ITrackingTransConnection> BeginTrackingTransConnection() {
        if (this._FuncBeginTransaction is not null) {
            return await this._FuncBeginTransaction(this);
        } else {
            return new TrackingTestTransaction();
        }
    }

    public void Dispose() {
    }
}

public class TrackingTestTransaction : TrackingTransConnection {
    public TrackingTestTransaction() {
    }

    public override Task CommitAsync() {
        return Task.CompletedTask;
    }
}
