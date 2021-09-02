using System;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public interface ISyncItemFactory<T> {
        Task<T> CreateItem(object id);
    }
}
