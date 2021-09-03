using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public sealed class SyncByIdUntyped : SyncById {
        private bool _ItemIsSet;
        private object? _Item;

        public SyncByIdUntyped(SyncByType syncByType, object id) : base(syncByType, id) { }

        protected override SyncLock CreateSyncLock(bool exclusiveLock) => new SyncLockUntyped(this, exclusiveLock);

        public override object GetItemUntyped() {
            if (this._ItemIsSet) {
                return _Item!;
            } else {
                throw new InvalidOperationException("Item is not set.");
            }
        }

        public override bool IsItemSet() => this._ItemIsSet;

        public override void SetItemUntyped(object item) {
            this._Item = item;
            this._ItemIsSet = true;
        }

        protected override async Task OnLock() {
            if (!this._ItemIsSet) {
                var syncByType = (SyncByType<object>)this.SyncByType;
                Monitor.Enter(this);
                try {
                    var item = await syncByType.SyncItemFactory.CreateItem(this.Id);
                    this.SetItemUntyped(item);
                } finally {
                    Monitor.Exit(this);
                }
            }
        }
        ~SyncByIdUntyped() {
            this.Dispose(disposing: false);
        }
    }
}
