
using System;
using System.Reflection;

namespace Brimborium.TypedStoredProcedure {
    public sealed record CalculateMappingReturnItem(
        int Mode,
        StoredProcedureResultColumn Column,
        string CsName,
        ParameterInfo? ctorPI,
        PropertyInfo? Writeable,
        PropertyInfo? Readable,
        Type ValueType,
        string csReadCall) {
        public string Identity => this.Column.Name;
        public int Index => this.Column.Index;
    }
}
