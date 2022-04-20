using System;
using System.Threading.Tasks;

namespace Brimborium.Tracking;

public class TrackingTestConnection : TrackingConnection {
    private readonly Func<TrackingTestConnection, TrackingTestTransaction>? _FuncBeginTransaction;

    public TrackingTestConnection(
        Func<TrackingTestConnection, TrackingTestTransaction>? funcBeginTransaction
        ) {
        this._FuncBeginTransaction = funcBeginTransaction;
    }

    public override TrackingTransaction BeginTransaction() {
        if (this._FuncBeginTransaction is not null) {
            return this._FuncBeginTransaction(this);
        } else {
            return new TrackingTestTransaction();
        }
    }
}

public class TrackingTestTransaction : TrackingTransaction {
    public TrackingTestTransaction() {
    }

    public override Task CommitAsync() {
        return Task.CompletedTask;
    }
}
