namespace Brimborium.CodeGeneration;
public enum NullableKind { 
    ReferenceType,
    Nullable,
    ValueType
}
public static class TypeExtenstions {

    public static NullableKind GetNullableKind(this Type type) {
        if (type.IsClass) {
            return NullableKind.ReferenceType;
        }
        if (System.Nullable.GetUnderlyingType(type) is null) {
            return NullableKind.Nullable;
        }
        return NullableKind.ValueType;
    }

    public static Type AsNullable(this Type type) {
        if (type.IsClass) {
            return type;
        }
        if (System.Nullable.GetUnderlyingType(type) is null) {            
            return typeof(System.Nullable<>).MakeGenericType(type);
        } 
        return type;
    }

    public static Type AsNonNullable(this Type type) {
        if (System.Nullable.GetUnderlyingType(type) is Type underlyingType) {
            return underlyingType;
        } else {
            return type;
        }
    }

    public static string AsCSharp(this Type type) {
        if (System.Nullable.GetUnderlyingType(type) is Type underlyingType) {
            return underlyingType.AsCSharp() + "?";
        }
        return type.FullName!;
    }
}
