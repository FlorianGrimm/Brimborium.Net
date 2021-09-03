using System;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public class SyncLock : ISyncLock, IDisposable {
        protected SyncLockMode _LockMode;
        protected bool _DisposedValue;
        protected readonly SyncById _SyncById;
        protected SyncGroupLock? _SyncGroupLock;

        protected SyncLock(SyncById item) {
            this._LockMode = SyncLockMode.Initialized;
            this._SyncById = item;
        }

        internal void SetWaiting() {
            lock (this) {
                this._LockMode = SyncLockMode.Waiting;
            }
        }

        internal bool SetActive(SyncGroupLock syncGroupLock) {
            lock (this) {
                if (this._LockMode == SyncLockMode.Initialized) {
                    this._SyncGroupLock = syncGroupLock;
                    this._LockMode = SyncLockMode.Active;
                    return true;
                } else if (this._LockMode == SyncLockMode.Waiting) {
                    this._SyncGroupLock = syncGroupLock;
                    this._LockMode = SyncLockMode.Active;
                    return true;
                } else {
                    return false;
                }
            }
        }

        internal bool SetFinish() {
            lock (this) {
                if (this._LockMode != SyncLockMode.Finished) {
                    this._LockMode = SyncLockMode.Finished;
                    return true;
                } else {
                    return false;
                }
            }
        }

        internal bool IsFinished() => this._LockMode == SyncLockMode.Finished;

        protected void Dispose(bool disposing) {
            if (!this._DisposedValue) {
                this._DisposedValue = true;
                this._LockMode = SyncLockMode.Finished;
                this._SyncGroupLock?.Release(this);
            }
        }

        ~SyncLock() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public ISyncById GetSyncByIdUntyped() {
            if (this._DisposedValue) {
                throw new ObjectDisposedException("SyncLock");
            } else {
                return this._SyncById;
            }
        }
    }

    internal sealed class SyncLock<T> : SyncLock, ISyncLock<T>, IDisposable {
        internal SyncLock(ISyncById<T> syncById) : base((SyncById)syncById) { }

        public ISyncById<T> GetSyncById() {
            if (this._DisposedValue) {
                throw new ObjectDisposedException("SyncLock");
            } else {
                return (ISyncById<T>)this._SyncById;
            }
        }

        ~SyncLock() {
            this.Dispose(disposing: false);
        }
    }

    internal sealed class SyncLockUntyped : SyncLock {
        internal SyncLockUntyped(SyncById syncById) : base(syncById) { }

        ~SyncLockUntyped() {
            this.Dispose(disposing: false);
        }
    }
}
