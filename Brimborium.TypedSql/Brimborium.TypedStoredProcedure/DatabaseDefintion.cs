
using System;
using System.Collections.Generic;
using System.Linq;

namespace Brimborium.TypedStoredProcedure {
    public sealed record DatabaseDefintion(
            StoredProcedureDefintion[] StoredProcedures,
            TypePropertyNames[] IgnoreTypePropertyNames
        ) {
        public DictIgnoreTypePropertyNames GetIgnoreTypePropertyNames() {
            var result = new DictIgnoreTypePropertyNames();
            foreach (var tpn in this.IgnoreTypePropertyNames) {
                result[tpn.type] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
            foreach (var tpn in this.IgnoreTypePropertyNames) {
                var hs = result[tpn.type];
                foreach (var pn in tpn.propertyNames) {
                    hs.Add(pn);
                }
            }
            return result;
        }
        public DictGetStoredProcedureNames GetStoredProcedureNames() {
            var result = new DictGetStoredProcedureNames();

            foreach (var storedProcedure in this.StoredProcedures.Where(sp => sp.Enabled)) {
                result[storedProcedure.SqlName] = $"Execute{storedProcedure.Name}";
            }

            var keys = result.Keys.ToList();
            foreach (var key in keys) {
                if (key.EndsWith("_V2]")) {
                    var loweredKey = key.Replace("_V2]", "]");
                    if (result.ContainsKey(loweredKey)) {
                        // skip
                    } else {
                        var value = result[key];
                        value = value.Substring(0, value.Length - 3);
                        result[key] = value;
                    }
                }
            }

            return result;
        }
    }
}
