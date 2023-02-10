namespace Brimborium.BaseMethods;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class EquatableAttribute : Attribute {
    /// <summary>
    /// IEquatable will only be generated for explicitly defined attributes.
    /// </summary>
    public bool Explicit { get; set; }

    /// <summary>
    /// Equality and hash code do not consider members of base classes.
    /// </summary>
    public bool IgnoreInheritedMembers { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public class DefaultEqualityAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class OrderedEqualityAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.Property)]
public class IgnoreEqualityAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.Property)]
public class UnorderedEqualityAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.Property)]
public class ReferenceEqualityAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.Property)]
public class SetEqualityAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.Property)]
public class CustomEqualityAttribute : Attribute {
    public Type EqualityType { get; }
    public string FieldOrPropertyName { get; }

    public CustomEqualityAttribute(Type equalityType, string fieldOrPropertyName = "Default") {
        EqualityType = equalityType;
        FieldOrPropertyName = fieldOrPropertyName;
    }
}
