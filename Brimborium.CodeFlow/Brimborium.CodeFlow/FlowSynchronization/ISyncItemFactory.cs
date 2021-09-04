using System;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public interface ISyncItemFactory<T> {
        Task<IState<T>> CreateStateItem(IIdentity id);
    }

    public sealed class SyncItemFactoryFunction<T> : ISyncItemFactory<T> {
        private readonly Func<IIdentity, IState<T>> _CreateItem;

        public SyncItemFactoryFunction(
            Func<IIdentity, IState<T>> createItem
            ) {
            this._CreateItem = createItem;
        }
        public Task<IState<T>> CreateStateItem(IIdentity id) {
            return Task.FromResult<IState<T>>(this._CreateItem(id));
        }
    }

    public sealed class SyncItemFactoryFunctionAsync<T> : ISyncItemFactory<T> {
        private readonly Func<IIdentity, Task<IState<T>>> _CreateItem;

        public SyncItemFactoryFunctionAsync(
            Func<IIdentity, Task<IState<T>>> createItem
            ) {
            this._CreateItem = createItem;
        }

        public Task<IState<T>> CreateStateItem(IIdentity id) {
            return this._CreateItem(id);
        }
    }
}
