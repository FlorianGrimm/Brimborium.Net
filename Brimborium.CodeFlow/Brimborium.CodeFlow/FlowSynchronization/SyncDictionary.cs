using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public record SyncDictionaryOptions(
        SyncFactory Factory,
        bool AllowUnregistered,
        TimeSpan DefaultLifeTimeSpan
    );
    public class SyncDictionary : IDisposable {
        private readonly SyncDictionaryOptions _Options;
        private readonly SyncTimer? _SyncTimer;
        private readonly ConcurrentDictionary<Type, SyncByType> _SyncByType;
        private bool _DisposedValue;

        public SyncDictionary(
            SyncDictionaryOptions options,
            SyncTimer? syncTimer
            ) : base() {
            this._SyncByType = new ConcurrentDictionary<Type, SyncByType>();
            this._Options = options;
            this._SyncTimer = syncTimer;
        }

        internal SyncDictionaryOptions Options => this._Options;

        public SyncByType GetSyncByType(Type type) {
            {
                if (this._SyncByType.TryGetValue(type, out var syncByType)) {
                    return syncByType;
                }
            }
            while (true) {
                if (!this._SyncByType.TryGetValue(type, out var syncByType)) {
                    if (this.Options.AllowUnregistered) {
                        ISyncItemFactory<object> syncFactory = this.Options.Factory.GetSyncFactory(type);
                        syncByType = this.Options.Factory.CreateSyncByType<object>(this, type, syncFactory, this.Options.DefaultLifeTimeSpan);
                        if (!this._SyncByType.TryAdd(type, syncByType)) {
                            syncByType.Dispose();
                            continue;
                        }
                    } else {
                        throw new ArgumentException($"Type ${type.FullName} not allowed");
                    }
                }
                return syncByType;
            }
        }

        public async Task<ISyncLock<T>> LockAsync<T>(
            IIdentity id,
            bool exclusiveLock,
            SyncLockCollection? synLockCollection,
            CancellationToken cancellationToken = default) {
            var syncByType = this.GetSyncByType(typeof(T));
            var result = await syncByType.LockAsync(id, exclusiveLock, synLockCollection, cancellationToken);
            return (ISyncLock<T>)result;
        }

        public Task<ISyncLock> LockByTypeAsync(
            Type type,
            IIdentity id,
            bool exclusiveLock,
            SyncLockCollection? synLockCollection,
            CancellationToken cancellationToken = default) {
            var syncByType = this.GetSyncByType(type);
            return syncByType.LockAsync(id, exclusiveLock, synLockCollection, cancellationToken);
        }

#pragma warning disable IDE0060 // Remove unused parameter
        private void Dispose(bool disposing) {
#pragma warning restore IDE0060 // Remove unused parameter
            if (!this._DisposedValue) {
                // if (disposing) { }
                var arrKeys = this._SyncByType.Keys.ToArray();
                foreach (var key in arrKeys) {
                    if (this._SyncByType.TryRemove(key, out var syncById)) {
                        syncById.Dispose();
                    }
                }
                this._DisposedValue = true;
            }
        }

        ~SyncDictionary() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public bool RegisterType<T>(ISyncItemFactory<T> creator, TimeSpan? lifeTimeSpan) {
            while (true) {
                if (!this._SyncByType.ContainsKey(typeof(T))) {
                    var syncByType = this.Options.Factory.CreateSyncByType<T>(this, typeof(T), creator, lifeTimeSpan ?? this._Options.DefaultLifeTimeSpan);
                    if (!this._SyncByType.TryAdd(typeof(T), syncByType)) {
                        syncByType.Dispose();
                        continue;
                    }
                    return true;
                }
                return false;
            }
        }

        internal bool StopTimeoutDispose(SyncById syncById) {
            if (this._SyncTimer is null) {
                return false;
            } else {
                return true;
            }
        }

        internal bool StartTimeoutDispose(SyncById syncById, DateTime at) {
            if (this._SyncTimer is null) {
                return false;
            } else {
                return true;
            }
        }
    }

}
