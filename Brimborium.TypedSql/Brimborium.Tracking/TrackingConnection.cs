namespace Brimborium.Tracking;

public interface ITrackingConnection : IDisposable {
    Task<ITrackingTransConnection> BeginTrackingTransConnection();
}

public interface ITrackingTransConnection : IDisposable {
    Task CommitAsync();
}

public interface ITrackingAccessConnectionFactory {
    TrackingSqlConnectionOption GetOptions();
    void SetOptions(TrackingSqlConnectionOption value);

    Task<ITrackingTransConnection> BeginTrackingTransConnection(
        CancellationToken cancellationToken = default(CancellationToken)
        );
}

public abstract class TrackingTransConnection
    : ITrackingTransConnection
    , IDisposable {
    protected TrackingTransConnection() {
    }

    private bool _IsDisposed;

    public abstract Task CommitAsync();

    protected virtual bool Dispose(bool disposing) {
        if (_IsDisposed) {
            return false;
        } else {
            this._IsDisposed = true;
            System.GC.SuppressFinalize(this);
            return true;
        }
    }

    ~TrackingTransConnection() {
        this.Dispose(disposing: false);
    }

    public void Dispose() {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
