using System;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public interface ISyncItemFactory<T> {
        Task<T> CreateItem(object id);
#warning xxx
        //public SyncLock<T> CreateSyncLock(ISyncById<T> syncById) {
        //    return new SyncLock<T>(syncById);
        //}
    }

    public sealed class SyncItemFactoryFunction<T> : ISyncItemFactory<T> {
        private readonly Func<object, T> _CreateItem;

        public SyncItemFactoryFunction(
            Func<object, T> createItem
            ) {
            this._CreateItem = createItem;
        }
        public Task<T> CreateItem(object id) {
            return Task.FromResult<T>(this._CreateItem(id));
        }
    }

    public sealed class SyncItemFactoryFunctionAsync<T> : ISyncItemFactory<T> {
        private readonly Func<object, Task<T>> _CreateItem;

        public SyncItemFactoryFunctionAsync(
            Func<object, Task<T>> createItem
            ) {
            this._CreateItem = createItem;
        }

        public Task<T> CreateItem(object id) {
            return this._CreateItem(id);
        }
    }
}
