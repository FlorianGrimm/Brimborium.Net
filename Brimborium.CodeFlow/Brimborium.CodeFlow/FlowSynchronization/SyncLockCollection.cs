using System;
using System.Collections.Generic;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public sealed class SyncLockCollection : IDisposable {
        private bool _DisposedValue;
        private List<SyncLock> _SynLocks;
        public SyncLockCollection()
        {
            this._SynLocks = new List<SyncLock>();
        }

        private void Dispose(bool disposing)
        {
            if (!this._DisposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                this._DisposedValue = true;
            }
        }

        ~SyncLockCollection()
        {
            this.Dispose(disposing: false);
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal void Add(SyncLock result)
        {
            lock (this)
            {
                this._SynLocks.Add(result);
            }
        }
    }

}
