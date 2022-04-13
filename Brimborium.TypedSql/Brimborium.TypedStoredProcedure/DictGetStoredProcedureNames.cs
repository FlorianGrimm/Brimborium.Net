
using System;
using System.Collections.Generic;

namespace Brimborium.TypedStoredProcedure {
    public class DictGetStoredProcedureNames : Dictionary<string, string> {
        public DictGetStoredProcedureNames() : base(
                StringComparer.OrdinalIgnoreCase
            ) {
        }
    }
}
