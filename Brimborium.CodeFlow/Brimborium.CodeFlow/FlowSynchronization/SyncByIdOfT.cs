using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public sealed class SyncById<T> : SyncById, ISyncById<T> {
        private IState<T>? _Item;
        private bool _ItemIsSet;

        public SyncById(SyncByType<T> syncByType, IIdentity id) : base(syncByType, id) { }

        protected override SyncLock CreateSyncLock(bool exclusiveLock) => new SyncLock<T>(this, exclusiveLock);

        public bool IsStateSet() => this._ItemIsSet;

        public void SetState(IState<T> item) {
            this._Item = item;
            this._ItemIsSet = true;
        }

        public IState<T> GetState() {
            if (this._ItemIsSet) {
                return _Item!;
            } else {
                throw new InvalidOperationException("Item is not set.");
            }
        }

        protected override async Task OnLock() {
            if (!this._ItemIsSet) {
                var syncByType = (SyncByType<T>)this.SyncByType;
                Monitor.Enter(this);
                try {
                    var state = await syncByType.SyncItemFactory.CreateStateItem(this.Id);
                    this.SetState(state);
                } finally {
                    Monitor.Exit(this);
                }
            }
        }


        ~SyncById() {
            this.Dispose(disposing: false);
        }
    }
}
