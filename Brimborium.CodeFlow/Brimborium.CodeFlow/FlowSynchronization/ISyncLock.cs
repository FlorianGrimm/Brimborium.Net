using System;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public interface ISyncLock : IDisposable {
        ISyncById GetSyncByIdUntyped();
    }

    public interface ISyncLock<T> : IDisposable {
        ISyncById<T> GetSyncById();
    }
}
