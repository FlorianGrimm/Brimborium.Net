using System;
using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public class CBContext : CBValue {
        public CBContext() : base() {
            this.Item = new List<CBValue>();
        }

        public CBContext(string name) : base(name) {
            this.Item = new List<CBValue>();
        }

        public List<CBValue> Item { get; }

        public bool Add(CBValue cbValue) {
            int index = this.Item.BinarySearch(cbValue, CBValueNameComparer.GetInstance());
            if (index < 0) {
                this.Item.Insert(~index, cbValue);
                cbValue.SetContext(this);
                return true;
            } else {
                return false;
            }
        }

        public bool Remove(string name) {
            int index = this.Item.BinarySearch(new CBValue(name), CBValueNameComparer.GetInstance());
            if (index >= 0) {
                this.Item.RemoveAt(index);
                return true;
            } else {
                return false;
            }
        }

        public bool Remove(CBValue cbValue) {
            int index = this.Item.BinarySearch(cbValue, CBValueNameComparer.GetInstance());
            if (index >= 0) {
                this.Item.RemoveAt(index);
                return true;
            } else {
                return false;
            }
        }

        public int FindIndex(string name) {
            return this.Item.BinarySearch(new CBValue(name), CBValueNameComparer.GetInstance());
        }

        public CBValue<T> SetValue<T>(string name, T value) {
            var cbValue = new CBValue<T>(name, value);
            int index = this.Item.BinarySearch(cbValue, CBValueNameComparer.GetInstance());
            if (index < 0) {
                this.Item.Insert(~index, cbValue);
                cbValue.SetContext(this);
            } else {
                this.Item[index] = cbValue;
                cbValue.SetContext(this);
            }
            return cbValue;
        }
    }
}
