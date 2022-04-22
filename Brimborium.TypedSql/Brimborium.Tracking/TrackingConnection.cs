using System;
using System.Threading.Tasks;

namespace Brimborium.Tracking;

public abstract class TrackingConnection {
    protected TrackingConnection() {

    }
    public abstract TrackingTransConnection BeginTransaction();
}

public abstract class TrackingTransConnection : IDisposable {
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
