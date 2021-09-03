using System;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public interface ISyncLock : IDisposable {
        object GetItemUntyped();
        bool IsItemSet();
        void SetItemUntyped(object item);
    }

    public interface ISyncLock<T> : IDisposable {
        T GetItem();
        bool IsItemSet();
        void SetItem(T item);
    }
}
