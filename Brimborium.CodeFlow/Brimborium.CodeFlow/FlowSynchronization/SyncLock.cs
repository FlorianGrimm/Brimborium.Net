using System;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public class SyncLock : IDisposable, ISyncLock {
        protected SyncLockMode _LockMode;
        protected bool _DisposedValue;
        protected readonly SyncById _SyncById;
        protected SyncGroupLock? _SyncGroupLock;

        public bool ExclusiveLock { get; }

        protected SyncLock(SyncById item, bool exclusiveLock) {
            this._LockMode = SyncLockMode.Initialized;
            this._SyncById = item;
            this.ExclusiveLock = exclusiveLock;
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

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0060 // Remove unused parameter
        protected void Dispose(bool disposing) {
#pragma warning restore IDE0079 // Remove unnecessary suppression
#pragma warning restore IDE0060 // Remove unused parameter
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

        public object GetItemUntyped() {
            if (this._DisposedValue) {
                throw new ObjectDisposedException("SyncLock");
            } else {
                return this._SyncById.GetItemUntyped();
            }
        }

        public bool IsItemSet() {
            if (this._DisposedValue) {
                throw new ObjectDisposedException("SyncLock");
            } else {
                return this._SyncById.IsItemSet();

            }
        }

        public void SetItemUntyped(object item) {
            if (this._DisposedValue) {
                throw new ObjectDisposedException("SyncLock");
            } else if (this._SyncById.IsItemSet() && !this.ExclusiveLock) {
                throw new InvalidOperationException("No ExclusiveLock");
            } else {
                this._SyncById.SetItemUntyped(item);
            }
        }
    }

    internal sealed class SyncLock<T> : SyncLock, ISyncLock<T>, IDisposable {
        internal SyncLock(ISyncById<T> syncById, bool exclusiveLock) : base((SyncById)syncById, exclusiveLock) { }

        public T GetItem() {
            if (this._DisposedValue) {
                throw new ObjectDisposedException("SyncLock");
            } else {
                return ((ISyncById<T>)this._SyncById).GetItem();
            }
        }

        public void SetItem(T item) {
            if (this._DisposedValue) {
                throw new ObjectDisposedException("SyncLock");
            } else if (this._SyncById.IsItemSet() && !this.ExclusiveLock){
                throw new InvalidOperationException("No ExclusiveLock");
            } else {
                ((ISyncById<T>)this._SyncById).SetItem(item);
            }
        }

        ~SyncLock() {
            this.Dispose(disposing: false);
        }
    }

    internal sealed class SyncLockUntyped : SyncLock, ISyncLock {
        internal SyncLockUntyped(SyncById syncById, bool exclusiveLock) : base(syncById, exclusiveLock) { }

        ~SyncLockUntyped() {
            this.Dispose(disposing: false);
        }
    }
}
