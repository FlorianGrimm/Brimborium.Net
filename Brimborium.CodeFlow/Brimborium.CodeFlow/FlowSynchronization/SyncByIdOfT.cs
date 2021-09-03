using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public sealed class SyncById<T> : SyncById, ISyncById<T> {
        private T? _Item;
        private bool _ItemIsSet;

        public SyncById(SyncByType<T> syncByType, object id) : base(syncByType, id) { }

        protected override SyncLock CreateSyncLock(bool exclusiveLock) => new SyncLock<T>(this, exclusiveLock);

        public override bool IsItemSet() => this._ItemIsSet;

        public void SetItem(T item) {
            this._Item = item;
            this._ItemIsSet = true;
        }

        public override void SetItemUntyped(object item) {
            this.SetItem((T)item);
        }

        public override object GetItemUntyped() {
            if (this._ItemIsSet) {
                return _Item!;
            } else {
                throw new InvalidOperationException("Item is not set.");
            }
        }
        public T GetItem() {
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
                    var item = await syncByType.SyncItemFactory.CreateItem(this.Id);
                    this.SetItem(item);
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
