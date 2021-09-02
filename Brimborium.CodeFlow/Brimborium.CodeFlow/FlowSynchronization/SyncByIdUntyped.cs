namespace Brimborium.CodeFlow.FlowSynchronization {
    public class SyncByIdUntyped : SyncById {
        public SyncByIdUntyped(SyncByType syncByType) : base(syncByType)
        {

        }

        ~SyncByIdUntyped() {
            this.Dispose(disposing: false);
        }
    }

}
