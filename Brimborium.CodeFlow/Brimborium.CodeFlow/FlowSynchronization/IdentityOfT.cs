using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    //public  class Identity<T> : IIdentity {

    //    public Identity(T value) {
    //        this.Value = value;
    //    }

    //    public T Value { get; }
    //}

    public record Identity<T>(T Value) : IIdentity;
}
