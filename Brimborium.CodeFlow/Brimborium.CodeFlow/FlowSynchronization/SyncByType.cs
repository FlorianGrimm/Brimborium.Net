using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public abstract class SyncByType : IDisposable {
        private readonly ConcurrentDictionary<object, SyncById> _SyncById;
        private readonly SyncDictionary _ParentSyncDictionary;
        private bool _DisposedValue;

        protected SyncByType(SyncDictionary syncDictionary, Type byType) {
            this._ParentSyncDictionary = syncDictionary;
            this.ByType = byType;
            this._SyncById = new ConcurrentDictionary<object, SyncById>();
        }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0079 // Remove unnecessary suppression
        private void Dispose(bool disposing) {
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore IDE0079 // Remove unnecessary suppression
            if (!this._DisposedValue) {
                // if (disposing) { }
                var arrKeys = this._SyncById.Keys.ToArray();
                foreach (var key in arrKeys) {
                    if (this._SyncById.TryRemove(key, out var syncById)) {
                        syncById.Dispose();
                    }
                }
                this._DisposedValue = true;
            }
        }

        ~SyncByType() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Task<ISyncLock> LockAsync(
            IIdentity id,
            bool exclusiveLock,
            SyncLockCollection? synLockCollection,
            CancellationToken cancellationToken = default) {
            var syncById = this.GetSyncById(id);
            return syncById.LockAsync(exclusiveLock, synLockCollection, cancellationToken);
        }

        public SyncDictionary ParentSyncDictionary => this._ParentSyncDictionary;

        public SyncDictionaryOptions Options => this._ParentSyncDictionary.Options;

        public SyncFactory Factory => this._ParentSyncDictionary.Options.Factory;

        public Type ByType { get; }

        public SyncById GetSyncById(IIdentity id) {
            while (true) {
                if (!this._SyncById.TryGetValue(id, out var result)) {
                    lock (this) {
                        result = this.CreateSyncById(id);
                        if (this._SyncById.TryAdd(id, result)) {
                            return result;
                        } else {
                            result.Dispose();
                            continue;
                        }
                    }
                }
                return result;
            }
        }

        protected abstract SyncById CreateSyncById(IIdentity id);

        internal void StopTimeoutDispose(SyncById syncById) {
#warning throw new NotImplementedException();
            //if (this._ParentSyncDictionary.StopTimeoutDispose(this)) {             }
        }

        internal void StartTimeoutDispose(SyncById syncById) {
#warning throw new NotImplementedException();
            //if (this._ParentSyncDictionary.StartTimeoutDispose(this)) {             }
        }
    }
}
