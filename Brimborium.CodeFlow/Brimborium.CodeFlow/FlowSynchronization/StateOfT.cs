using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public class State<T> : IState<T> {
        private T? _Value;
        private bool _IsValueSet;

        public State() {
        }

        public State(T value) {
            this._Value = value;
            this._IsValueSet = true;
        }

        public bool GetIsValueSet => this._IsValueSet;

        public T GetValue() {
            if (this._IsValueSet) {
                return this._Value!;
            } else {
                throw new InvalidOperationException("Value is not set.");
            }
        }
        public void SetValue(T value) {
            this._Value = value;
            this._IsValueSet = true;
        }

        public override string ToString() {
            if (this._IsValueSet) {
                return this._Value?.ToString() ?? string.Empty;
            } else { 
                return "{State}";
            }
        }
    }
}
