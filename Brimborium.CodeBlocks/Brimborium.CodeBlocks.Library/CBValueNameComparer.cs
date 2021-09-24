using System;
using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public class CBValueNameComparer : IComparer<CBValue> {
        private static CBValueNameComparer? _Instance;
        public static CBValueNameComparer GetInstance() => (_Instance ??= new CBValueNameComparer());

        private readonly StringComparer _StringComparer;

        public CBValueNameComparer() {
            this._StringComparer = StringComparer.OrdinalIgnoreCase;
        }

        public int Compare(CBValue? x, CBValue? y) {
            if (ReferenceEquals(x, y)) { return 0; }
            if (x is null) { return -1; }
            if (y is null) { return +1; }
            return this._StringComparer.Compare(x.Name, y.Name);
        }
    }
}
