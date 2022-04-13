
using System.Reflection;

namespace Brimborium.TypedStoredProcedure {
    public class CalculateMappingProperty {
        public CalculateMappingProperty(string csName) {
            this.CsName = csName;
            this.Matched = false;
            this.CtorPI = null;
            this.Readable = null;
            this.Writeable = null;
        }

        public string CsName { get; set; }
        public bool Matched { get; set; }
        public bool Ignore { get; set; }
        public ParameterInfo? CtorPI { get; set; }
        public PropertyInfo? Readable { get; set; }
        public PropertyInfo? Writeable { get; set; }
    }
}
