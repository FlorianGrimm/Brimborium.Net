namespace Brimborium.SourceGenerator.BaseMethods;

internal class EqualityGeneratorBase {
    internal const string GeneratedCodeAttributeDeclaration = "[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Brimborium.BaseMethods\", \"1.0.0.0\")]";

    internal const string EnableNullableContext = "#nullable enable";

    // CS0612: Obsolete with no comment
    // CS0618: obsolete with comment
    internal const string SuppressObsoleteWarningsPragma = "#pragma warning disable CS0612,CS0618";

    internal static readonly string[] EqualsOperatorCodeComment = @"
/// <summary>
/// Indicates whether the object on the left is equal to the object on the right.
/// </summary>
/// <param name=""left"">The left object</param>
/// <param name=""right"">The right object</param>
/// <returns>true if the objects are equal; otherwise, false.</returns>".ToLines();

    internal static readonly string[] NotEqualsOperatorCodeComment = @"
/// <summary>
/// Indicates whether the object on the left is not equal to the object on the right.
/// </summary>
/// <param name=""left"">The left object</param>
/// <param name=""right"">The right object</param>
/// <returns>true if the objects are not equal; otherwise, false.</returns>".ToLines();

    internal const string InheritDocComment = "/// <inheritdoc/>";

    public static void BuildPropertyEquality(
        EquatableInformationType equatableInformationType,
        EquatableInformationProperty equatableInformationProperty,
        StringBuilder sb, 
        int level
        ) {
        if (equatableInformationProperty.PropertySymbol is null) {
            return;
        }
        IPropertySymbol property = equatableInformationProperty.PropertySymbol;

        if (equatableInformationProperty.IgnoreEquality) {
            return;
        }

        var propertyName = property.ToFQF();

        var typeName = property.Type.ToNullableFQF();

        if (equatableInformationProperty.UnorderedEquality) {
            var types = property.GetIDictionaryTypeArguments();

            if (types != null) {
                sb.AppendLine(level,
                    $"&& global::Brimborium.BaseMethods.DictionaryEqualityComparer<{string.Join(", ", types.Value)}>.Default.Equals({propertyName}!, other.{propertyName}!)");
            } else {
                types = property.GetIEnumerableTypeArguments()!;
                sb.AppendLine(level,
                    $"&& global::Brimborium.BaseMethods.UnorderedEqualityComparer<{string.Join(", ", types.Value)}>.Default.Equals({propertyName}!, other.{propertyName}!)");
            }
        } else if (equatableInformationProperty.OrderedEquality) {
            var types = property.GetIEnumerableTypeArguments()!;

            sb.AppendLine(level,
                $"&& global::Brimborium.BaseMethods.OrderedEqualityComparer<{string.Join(", ", types.Value)}>.Default.Equals({propertyName}!, other.{propertyName}!)");
        } else if (equatableInformationProperty.ReferenceEquality) {
            sb.AppendLine(level,
                $"&& global::Brimborium.BaseMethods.ReferenceEqualityComparer<{typeName}>.Default.Equals({propertyName}!, other.{propertyName}!)");
        } else if (equatableInformationProperty.SetEquality) {
            var types = property.GetIEnumerableTypeArguments()!;

            sb.AppendLine(level,
                $"&& global::Brimborium.BaseMethods.SetEqualityComparer<{string.Join(", ", types.Value)}>.Default.Equals({propertyName}!, other.{propertyName}!)");
        } else if (equatableInformationProperty.GetCustomEquality()) {
            var comparerType = equatableInformationProperty.EqualityType!;
            var comparerMemberName = equatableInformationProperty.FieldOrPropertyName!;

            if (comparerType.GetMembers().Any(x => x.Name == comparerMemberName && x.IsStatic) || comparerType.GetProperties().Any(x => x.Name == comparerMemberName && x.IsStatic)) {
                sb.AppendLine(level,
                    $"&& {comparerType.ToFQF()}.{comparerMemberName}.Equals({propertyName}!, other.{propertyName}!)");
            } else {
                sb.AppendLine(level,
                    $"&& new {comparerType.ToFQF()}().Equals({propertyName}!, other.{propertyName}!)");
            }
        } else if (
              !(equatableInformationType.Explicit)
              || equatableInformationProperty.DefaultEquality) {
            sb.AppendLine(level,
                $"&& global::Brimborium.BaseMethods.DefaultEqualityComparer<{typeName}>.Default.Equals({propertyName}!, other.{propertyName}!)");
        }
    }

    public static void BuildPropertyHashCode(
        EquatableInformationType equatableInformationType,
        EquatableInformationProperty equatableInformationProperty,
        StringBuilder sb,
        int level) {
        if (equatableInformationProperty.PropertySymbol is null) {
            return;
        }
        IPropertySymbol property = equatableInformationProperty.PropertySymbol;
        
        if (equatableInformationProperty.IgnoreEquality) {
            return;
        }

        if (equatableInformationType.Explicit) {
            if (!equatableInformationProperty.DefaultEquality
                && !equatableInformationProperty.UnorderedEquality
                && !equatableInformationProperty.OrderedEquality
                && !equatableInformationProperty.ReferenceEquality
                && !equatableInformationProperty.SetEquality
                && !equatableInformationProperty.GetCustomEquality()
                ) {
                return;
            }
        }
        var propertyName = property.ToFQF();

        var typeName = property.Type.ToNullableFQF();

        sb.AppendLine(level, $"hashCode.Add(");
        level++;
        sb.AppendLine(level, $"this.{propertyName}!,");
        sb.AppendMargin(level);
        
        if (equatableInformationProperty.UnorderedEquality) {
            var types = property.GetIDictionaryTypeArguments();

            if (types != null) {
                sb.Append(
                    $"global::Brimborium.BaseMethods.DictionaryEqualityComparer<{string.Join(", ", types.Value)}>.Default");
            } else {
                types = property.GetIEnumerableTypeArguments()!;
                sb.Append(
                    $"global::Brimborium.BaseMethods.UnorderedEqualityComparer<{string.Join(", ", types.Value)}>.Default");
            }
        } else if (equatableInformationProperty.OrderedEquality) {
            var types = property.GetIEnumerableTypeArguments()!;
            sb.Append(
                $"global::Brimborium.BaseMethods.OrderedEqualityComparer<{string.Join(", ", types.Value)}>.Default");
        } else if (equatableInformationProperty.ReferenceEquality) {
            sb.Append($"global::Brimborium.BaseMethods.ReferenceEqualityComparer<{typeName}>.Default");
        } else if (equatableInformationProperty.SetEquality ) {
            var types = property.GetIEnumerableTypeArguments()!;
            sb.Append(
                $"global::Brimborium.BaseMethods.SetEqualityComparer<{string.Join(", ", types.Value)}>.Default");
        } else if (equatableInformationProperty.GetCustomEquality()) {
            var comparerType = equatableInformationProperty.EqualityType!;
            var comparerMemberName = equatableInformationProperty.FieldOrPropertyName!;
            
            if (comparerType.GetMembers().Any(x => x.Name == comparerMemberName && x.IsStatic) || comparerType.GetProperties().Any(x => x.Name == comparerMemberName && x.IsStatic)) {
                sb.Append($"{comparerType.ToFQF()}.{comparerMemberName}");
            } else {
                sb.Append($"new {comparerType.ToFQF()}()");
            }
        } else {
            //
            //sb.Append($"global::System.Collections.Generic.EqualityComparer<{typeName}>.Default");
            sb.Append($"global::Brimborium.BaseMethods.DefaultEqualityComparer<{typeName}>.Default");
        }

        sb.AppendLine(");");
    }
}