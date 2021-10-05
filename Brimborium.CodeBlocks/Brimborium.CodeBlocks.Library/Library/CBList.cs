using System.Collections;
using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public class CBList<T> : System.Collections.Generic.IList<T>
        where T : ICBCodeElement {
        private readonly ICBCodeElement _Owner;
        private readonly List<T> _Items;

        public CBList(ICBCodeElement owner) {
            this._Items = new List<T>();
            this._Owner = owner;
        }

        public T this[int index] {
            get { 
                return this._Items[index]; 
            }
            set {
                this._Items[index] = value;
                if (value.Parent is null) {
                    value.Parent = this._Owner;
                }
            }
        }

        public int Count => this._Items.Count;

        public bool IsReadOnly => false;

        public void AddRange(IEnumerable<T> items) {
            var cnt = this._Items.Count;
            this._Items.AddRange(items);
            for (int idx = cnt; idx < this._Items.Count; idx++) {
                var item = this._Items[idx];
                if (item.Parent is null) {
                    item.Parent = _Owner;
                }
            }
        }

        public void Add(T item) {
            ((ICollection<T>)this._Items).Add(item);
            if (item.Parent is null) {
                item.Parent = _Owner;
            }
        }

        public void Clear() {
            ((ICollection<T>)this._Items).Clear();
        }

        public bool Contains(T item) {
            return ((ICollection<T>)this._Items).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            ((ICollection<T>)this._Items).CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator() {
            return ((IEnumerable<T>)this._Items).GetEnumerator();
        }

        public int IndexOf(T item) {
            return ((IList<T>)this._Items).IndexOf(item);
        }

        public void Insert(int index, T item) {
            ((IList<T>)this._Items).Insert(index, item);
        }

        public bool Remove(T item) {
            return ((ICollection<T>)this._Items).Remove(item);
        }

        public void RemoveAt(int index) {
            ((IList<T>)this._Items).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)this._Items).GetEnumerator();
        }
    }
}
