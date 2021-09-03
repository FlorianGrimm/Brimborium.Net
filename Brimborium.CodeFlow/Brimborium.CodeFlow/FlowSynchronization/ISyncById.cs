using System;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public interface ISyncById: IDisposable {
        object GetItemUntyped();
        bool IsItemSet();
        void SetItemUntyped(object item);
    }
    public interface ISyncById<T> {
        T GetItem();
        bool IsItemSet();
        void SetItem(T item);
    }
}