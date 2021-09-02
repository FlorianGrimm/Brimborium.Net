using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Brimborium.CodeFlow
{
    public class Context : IContext {
        
    }


    public interface ISyncItemFactory<T> {
        Task<T> CreateItem(object id);
    }

    public sealed class SyncLockCollection : IDisposable {
        private bool _DisposedValue;
        private List<SyncLock> _SynLocks;
        public SyncLockCollection() {
            this._SynLocks = new List<SyncLock>();
        }

        private void Dispose(bool disposing) {
            if (!_DisposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _DisposedValue = true;
            }
        }

        ~SyncLockCollection() {
            Dispose(disposing: false);
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal void Add(SyncLock result) {
            lock (this) {
                this._SynLocks.Add(result);
            }
        }
    }

    public class SyncFactory {
        public SyncFactory() {
        }

        public virtual SyncByType CreateSyncByType(Type type, SyncDictionary syncDictionary, ISyncItemFactory<object> syncFactory) {
            return new SyncByType<object>(syncDictionary, syncFactory);
        }


        public virtual SyncById CreateSyncById(SyncByType syncByType) {
            return new SyncById(syncByType);
        }

        public virtual ISyncItemFactory<object> GetSyncFactory(Type type) {
            return new GenaralSyncItemFactory(type);
        }

        class GenaralSyncItemFactory : ISyncItemFactory<object> {
            private readonly Type _Type;

            public GenaralSyncItemFactory(Type type) {
                this._Type = type;
            }
            public Task<object> CreateItem(object id) {
                object result = System.Activator.CreateInstance(this._Type)
                    ?? throw new InvalidOperationException();
                return Task.FromResult<object>(result);
            }
        }
    }

    public class SyncDictionary : IDisposable {
        private ConcurrentDictionary<Type, SyncByType> _SyncByType;
        private bool _DisposedValue;
        private Func<Type, object, Task<object>> _Creator;
        private SyncFactory _SyncFactory;

        private static Task<object> DefaultCreator(Type type, object id) {
            var result = System.Activator.CreateInstance(type)!;
            return Task.FromResult<object>(result);
        }

        public SyncDictionary(SyncFactory? syncFactory = default) : base() {
            this._SyncByType = new ConcurrentDictionary<Type, SyncByType>();
            this._Creator = DefaultCreator;
            this._SyncFactory = syncFactory ?? new SyncFactory();
        }


        private SyncByType GetSyncByType(Type type) {
            while (true) {
                if (!this._SyncByType.TryGetValue(type, out var syncByType)) {
                    ISyncItemFactory<object> syncFactory = this._SyncFactory.GetSyncFactory(type);
                    syncByType = this._SyncFactory.CreateSyncByType(type, this, syncFactory);
                    if (!this._SyncByType.TryAdd(type, syncByType)) {
                        syncByType.Dispose();
                        continue;
                    }
                }
                return syncByType;
            }
        }


        public Task<IDisposable> LockAsync(Type type, string id, SyncLockCollection? synLockCollection, CancellationToken cancellationToken = default) {
            var syncByType = GetSyncByType(type);
            return syncByType.LockAsync(id, synLockCollection, cancellationToken);
        }

        private void Dispose(bool disposing) {
            if (!_DisposedValue) {
                // if (disposing) { }
                var arrKeys = this._SyncByType.Keys.ToArray();
                foreach (var key in arrKeys) {
                    if (this._SyncByType.TryRemove(key, out var syncById)) {
                        syncById.Dispose();
                    }
                }
                _DisposedValue = true;
            }
        }

        ~SyncDictionary() {
            Dispose(disposing: false);
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public bool Add<T>(ISyncItemFactory<T> creator) {
            while (true) {
                if (!this._SyncByType.TryGetValue(typeof(T), out var syncByType)) {
                    syncByType = new SyncByType(creator);
                    if (!this._SyncByType.TryAdd(typeof(T), syncByType)) {
                        syncByType.Dispose();
                        continue;
                    }
                    return true;
                }
                return false;
            }
        }

        public void SetDefaultCreator(Func<Type, object, Task<object>> creator) {
            this._Creator = creator;
        }

        internal void StopTimeoutDispose() {
        }

        internal void StartTimeoutDispose() {
        }
    }

    public abstract class SyncByType {
        private readonly ConcurrentDictionary<object, SyncById> _SyncById;
        private readonly SyncDictionary _ParentSyncDictionary;
        private bool _DisposedValue;

        protected SyncByType(SyncDictionary syncDictionary) {
            this._ParentSyncDictionary = syncDictionary;
            this._SyncById = new ConcurrentDictionary<object, SyncById>();
        }

        private void Dispose(bool disposing) {
            if (!_DisposedValue) {
                // if (disposing) { }
                var arrKeys = this._SyncById.Keys.ToArray();
                foreach (var key in arrKeys) {
                    if (this._SyncById.TryRemove(key, out var syncById)) {
                        syncById.Dispose();
                    }
                }
                _DisposedValue = true;
            }
        }

        ~SyncByType() {
            Dispose(disposing: false);
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Task<IDisposable> LockAsync(object id, SyncLockCollection? synLockCollection, CancellationToken cancellationToken = default) {
            var syncById = this.GetSyncById(id);
            return syncById.LockAsync(id, synLockCollection, cancellationToken);
        }

        public SyncDictionary GetParentSyncDictionary() => this._ParentSyncDictionary;

        private SyncById GetSyncById(object id) {
            while (true) {
                if (!this._SyncById.TryGetValue(id, out var result)) {
                    lock (this) {
                        result = this.CreateSyncById();
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

        protected abstract SyncById CreateSyncById();
    }

    public class SyncByType<T> : SyncByType {
        private readonly ISyncItemFactory<T> _SyncFactory;

        public SyncByType(SyncDictionary syncDictionary, ISyncItemFactory<T> syncFactory)
            : base(syncDictionary) {
            this._SyncFactory = syncFactory;
        }

        protected override SyncById CreateSyncById() {
            this._SyncFactory.CreateItem
        }
    }
    public class SyncById : IDisposable {
        private SemaphoreSlim? _SemaphoreSlim;
        internal readonly SyncByType SyncByType;
        private bool _DisposedValue;
        private SyncLock? _CurrentLock;

        public SyncById(SyncByType syncByType) {
            this.SyncByType = syncByType;
            this._CurrentLock = null;
        }

        public async Task<IDisposable> LockAsync(object id, SyncLockCollection? synLockCollection, CancellationToken cancellationToken = default) {
            var semaphoreSlim = this.GetSemaphoreSlim();
            await semaphoreSlim.WaitAsync(cancellationToken);
            var result = new SyncLock(this, id);
            this._CurrentLock = result;
            if (synLockCollection is not null) {
                synLockCollection.Add(result);
            }
            this.StopTimeoutDispose();
#warning here
            return result;
        }


        private SemaphoreSlim GetSemaphoreSlim() {
            if (this._SemaphoreSlim is null) {
                lock (this) {
                    if (this._SemaphoreSlim is null) {
                        var semaphoreSlim = new SemaphoreSlim(1, 1);
                        var oldValue = System.Threading.Interlocked.CompareExchange(ref this._SemaphoreSlim, semaphoreSlim, null);
                        if (ReferenceEquals(oldValue, null)) {
                            // OK 
                            System.Threading.Interlocked.MemoryBarrier();
                            return this._SemaphoreSlim;
                        } else {
                            semaphoreSlim.Dispose();
                            return this._SemaphoreSlim;
                        }
                    }
                }
            }
            return this._SemaphoreSlim;
        }

        // protected virtual
        private void Dispose(bool disposing) {
            if (!_DisposedValue) {
                if (disposing) {
                }
                using (var semaphoreSlim = this._SemaphoreSlim) {
                    this._SemaphoreSlim = null;
                }
                _DisposedValue = true;
            }
        }

        ~SyncById() {
            Dispose(disposing: false);
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal void Release(SyncLock syncLock) {
            var old = System.Threading.Interlocked.CompareExchange(ref this._CurrentLock, null, syncLock);
            if (ReferenceEquals(old, syncLock)) {
                var semaphore = this._SemaphoreSlim;
                if (semaphore is not null) {
                    semaphore.Release();
                    this.StartTimeoutDispose();
                }
            }
        }

        private void StopTimeoutDispose() {
            this.SyncByType.GetParentSyncDictionary().StopTimeoutDispose();
        }

        private void StartTimeoutDispose() {
            this.SyncByType.GetParentSyncDictionary().StartTimeoutDispose();
        }
    }

    public class SyncByIdTyped<T> : SyncById {
        public SyncByIdTyped(SyncByType syncByType) : base(syncByType) {

        }
    }
    public class SyncByIdUntyped : SyncById {
        public SyncByIdUntyped(SyncByType syncByType) : base(syncByType) {

        }
    }

    public sealed class SyncLock : IDisposable {
        private SyncById _SyncById;
        private object _Id;
        private bool _DisposedValue;

        public SyncLock(SyncById syncById, object id) {
            this._SyncById = syncById;
            this._Id = id;
        }

        private void Dispose(bool disposing) {
            if (!_DisposedValue) {
                this._SyncById.Release(this);
                _DisposedValue = true;
            }
        }

        ~SyncLock() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }


}
