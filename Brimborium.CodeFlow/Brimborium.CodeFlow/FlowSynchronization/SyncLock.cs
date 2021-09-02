using System;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public sealed class SyncLock : IDisposable {
        private SyncById _SyncById;
        private object _Id;
        private bool _DisposedValue;

        public SyncLock(SyncById syncById, object id)
        {
            this._SyncById = syncById;
            this._Id = id;
        }

        private void Dispose(bool disposing)
        {
            if (!this._DisposedValue)
            {
                this._SyncById.Release(this);
                this._DisposedValue = true;
            }
        }

        ~SyncLock()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: false);
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
