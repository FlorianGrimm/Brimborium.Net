using System;
using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public class CBContext : CBValue {
        public CBContext(CBContext? parent) : base() {
            this.Item = new List<CBValue>();
            this.Parent = parent;
        }

        public CBContext(string name) : base(name) {
            this.Item = new List<CBValue>();
        }

        public CBContext? Parent { get; set; }

        public List<CBValue> Item { get; }

        public bool Add(CBValue cbValue) {
            int index = this.FindIndex(cbValue, false);
            if (index < 0) {
                this.Item.Insert(~index, cbValue);
                cbValue.SetContext(this);
                return true;
            } else {
                this.Item.Insert(index + 1, cbValue);
                return false;
            }
        }

        public bool Remove(string name)
            => this.Remove(new CBValue(name));

        public bool Remove(CBValue cbValue) {
            int index = this.FindIndex(cbValue, true);
            if (index >= 0) {
                this.Item.RemoveAt(index);
                return true;
            } else {
                return false;
            }
        }

        public int FindIndex(string name, bool firstOrLast)
            => this.FindIndex(new CBValue(name), firstOrLast);

        public int FindIndex(CBValue cbName, bool firstOrLast) {
            var cmp = CBValueNameComparer.GetInstance();
            int index = this.Item.BinarySearch(cbName, cmp);
            if (index >= 0) {
                if (firstOrLast) {
                    while (0 < index) {
                        if (cmp.Compare(cbName, this.Item[index - 1]) == 0) {
                            index--;
                        }
                    }
                } else {
                    while ((index + 1) < this.Item.Count) {
                        if (cmp.Compare(cbName, this.Item[index + 1]) == 0) {
                            index++;
                        }
                    }
                }
            }
            return index;
        }

        public CBValue<T> SetValue<T>(string name, T value) {
            var cbValue = new CBValue<T>(name, value);
            int index = this.FindIndex(cbValue, true);
            if (index < 0) {
                this.Item.Insert(~index, cbValue);
                cbValue.SetContext(this);
            } else {
                this.Item[index] = cbValue;
                cbValue.SetContext(this);
            }
            return cbValue;
        }

        public List<CBValue> GetValue(string name) {
            var result = new List<CBValue>();
            var cbName = new CBValue(name);
            var cmp = CBValueNameComparer.GetInstance();
            GetValueRec(this);
            return result;

            void GetValueRec(CBContext context) {
                int index = context.FindIndex(cbName, true);
                if (index >= 0) {
                    do {
                        var item = context.Item[index];
                        if (cmp.Compare(item, cbName) == 0) {
                            result.Add(item);
                        } else {
                            break;
                        }
                    } while (index < context.Item.Count);
                    return;
                }
                {
                    if (context.Parent is not null) {
                        GetValueRec(context.Parent);
                    }
                }
            }
        }

        //private static CBValue? GetValue(CBValue name) {
        //    //return this.Item.BinarySearch(new CBValue(name), CBValueNameComparer.GetInstance());
        //    return null;
        //}

        public CBValue? GetValueT<T>(string name) {
            return null;
        }
    }
}
