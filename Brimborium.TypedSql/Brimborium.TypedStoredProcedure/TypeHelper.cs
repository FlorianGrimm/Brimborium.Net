
using System;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.TypedStoredProcedure {
    public static class TypeHelper {
        public static bool IsListOf(Type type, [MaybeNullWhen(false)] out Type itemType) {
            if (type.IsConstructedGenericType) {
                var gtd = type.GetGenericTypeDefinition();
                if (typeof(System.Collections.Generic.List<>).Equals(gtd)) {
                    var genericArguments = type.GetGenericArguments();
                    itemType = genericArguments[0];
                    return true;
                }
            }
            itemType = default;
            return false;
        }
    }
}
