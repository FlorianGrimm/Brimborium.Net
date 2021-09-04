using System;

namespace Brimborium.CodeFlow.FlowSynchronization {
    // convert to ISyncLock<T>
    public interface ISyncLock : IDisposable {
    }

    public interface ISyncLock<T> : IDisposable {
        IState<T> GetState();
        bool IsStateSet();
        void SetState(IState<T> item);
    }
}
