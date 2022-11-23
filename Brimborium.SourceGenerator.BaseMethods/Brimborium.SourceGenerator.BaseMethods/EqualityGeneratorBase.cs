namespace Brimborium.BaseMethods
{
    internal class EqualityGeneratorBase
    {
        protected const string GeneratedCodeAttributeDeclaration = "[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Brimborium.BaseMethods\", \"1.0.0.0\")]";

        internal const string EnableNullableContext = "#nullable enable";

        // CS0612: Obsolete with no comment
        // CS0618: obsolete with comment
        internal const string SuppressObsoleteWarningsPragma = "#pragma warning disable CS0612,CS0618";

        protected static readonly string[] EqualsOperatorCodeComment = @"
/// <summary>
/// Indicates whether the object on the left is equal to the object on the right.
/// </summary>
/// <param name=""left"">The left object</param>
/// <param name=""right"">The right object</param>
/// <returns>true if the objects are equal; otherwise, false.</returns>".ToLines();

        protected static readonly string[] NotEqualsOperatorCodeComment = @"
/// <summary>
/// Indicates whether the object on the left is not equal to the object on the right.
/// </summary>
/// <param name=""left"">The left object</param>
/// <param name=""right"">The right object</param>
/// <returns>true if the objects are not equal; otherwise, false.</returns>".ToLines();

        protected const string InheritDocComment = "/// <inheritdoc/>";

        public static void BuildPropertyEquality(AttributesMetadata attributesMetadata, StringBuilder sb, int level,
            IPropertySymbol property, bool explicitMode)
        {
            if (attributesMetadata.IgnoreEquality is not null && property.HasAttribute(attributesMetadata.IgnoreEquality))
                return;

            var propertyName = property.ToFQF();

            var typeName = property.Type.ToNullableFQF();

            if (attributesMetadata.UnorderedEquality is not null && property.HasAttribute(attributesMetadata.UnorderedEquality))
            {
                var types = property.GetIDictionaryTypeArguments();

                if (types != null)
                {
                    sb.AppendLine(level,
                        $"&& global::Brimborium.BaseMethods.DictionaryEqualityComparer<{string.Join(", ", types.Value)}>.Default.Equals({propertyName}!, other.{propertyName}!)");
                }
                else
                {
                    types = property.GetIEnumerableTypeArguments()!;
                    sb.AppendLine(level,
                        $"&& global::Brimborium.BaseMethods.UnorderedEqualityComparer<{string.Join(", ", types.Value)}>.Default.Equals({propertyName}!, other.{propertyName}!)");
                }
            }
            else if (attributesMetadata.OrderedEquality is not null && property.HasAttribute(attributesMetadata.OrderedEquality))
            {
                var types = property.GetIEnumerableTypeArguments()!;

                sb.AppendLine(level,
                    $"&& global::Brimborium.BaseMethods.OrderedEqualityComparer<{string.Join(", ", types.Value)}>.Default.Equals({propertyName}!, other.{propertyName}!)");
            }
            else if (attributesMetadata.ReferenceEquality is not null && property.HasAttribute(attributesMetadata.ReferenceEquality))
            {
                sb.AppendLine(level,
                    $"&& global::Brimborium.BaseMethods.ReferenceEqualityComparer<{typeName}>.Default.Equals({propertyName}!, other.{propertyName}!)");
            }
            else if (attributesMetadata.SetEquality is not null && property.HasAttribute(attributesMetadata.SetEquality))
            {
                var types = property.GetIEnumerableTypeArguments()!;

                sb.AppendLine(level,
                    $"&& global::Brimborium.BaseMethods.SetEqualityComparer<{string.Join(", ", types.Value)}>.Default.Equals({propertyName}!, other.{propertyName}!)");
            }
            else if (attributesMetadata.CustomEquality is not null && property.HasAttribute(attributesMetadata.CustomEquality))
            {
                var attribute = property.GetAttribute(attributesMetadata.CustomEquality);
                var comparerType = (INamedTypeSymbol) attribute?.ConstructorArguments[0].Value!;
                var comparerMemberName = (string) attribute?.ConstructorArguments[1].Value!;

                if (comparerType.GetMembers().Any(x => x.Name == comparerMemberName && x.IsStatic) || comparerType.GetProperties().Any(x => x.Name == comparerMemberName && x.IsStatic))
                {
                    sb.AppendLine(level,
                        $"&& {comparerType.ToFQF()}.{comparerMemberName}.Equals({propertyName}!, other.{propertyName}!)");
                }
                else
                {
                    sb.AppendLine(level,
                        $"&& new {comparerType.ToFQF()}().Equals({propertyName}!, other.{propertyName}!)");
                }
            }
            else if (
                !explicitMode ||
                attributesMetadata.DefaultEquality is not null && property.HasAttribute(attributesMetadata.DefaultEquality))
            {
                sb.AppendLine(level,
                    $"&& global::System.Collections.Generic.EqualityComparer<{typeName}>.Default.Equals({propertyName}!, other.{propertyName}!)");
            }
        }

        public static void BuildPropertyHashCode(
            IPropertySymbol property,
            AttributesMetadata attributesMetadata,
            StringBuilder sb,
            int level,
            bool explicitMode)
        {
            if (attributesMetadata.IgnoreEquality is not null && property.HasAttribute(attributesMetadata.IgnoreEquality))
                return;

            if (explicitMode &&
                !(attributesMetadata.DefaultEquality is not null && property.HasAttribute(attributesMetadata.DefaultEquality)) &&
                !(attributesMetadata.UnorderedEquality is not null && property.HasAttribute(attributesMetadata.UnorderedEquality)) &&
                !(attributesMetadata.OrderedEquality is not null && property.HasAttribute(attributesMetadata.OrderedEquality)) &&
                !(attributesMetadata.ReferenceEquality is not null && property.HasAttribute(attributesMetadata.ReferenceEquality)) &&
                !(attributesMetadata.SetEquality is not null && property.HasAttribute(attributesMetadata.SetEquality)) &&
                !(attributesMetadata.CustomEquality is not null && property.HasAttribute(attributesMetadata.CustomEquality)))
                return;

            var propertyName = property.ToFQF();

            var typeName = property.Type.ToNullableFQF();

            sb.AppendLine(level, $"hashCode.Add(");
            level++;
            sb.AppendLine(level, $"this.{propertyName}!,");
            sb.AppendMargin(level);

            if (attributesMetadata.UnorderedEquality is not null && property.HasAttribute(attributesMetadata.UnorderedEquality))
            {
                var types = property.GetIDictionaryTypeArguments();

                if (types != null)
                {
                    sb.Append(
                        $"global::Brimborium.BaseMethods.DictionaryEqualityComparer<{string.Join(", ", types.Value)}>.Default");
                }
                else
                {
                    types = property.GetIEnumerableTypeArguments()!;
                    sb.Append(
                        $"global::Brimborium.BaseMethods.UnorderedEqualityComparer<{string.Join(", ", types.Value)}>.Default");
                }
            }
            else if (attributesMetadata.OrderedEquality is not null && property.HasAttribute(attributesMetadata.OrderedEquality))
            {
                var types = property.GetIEnumerableTypeArguments()!;
                sb.Append(
                    $"global::Brimborium.BaseMethods.OrderedEqualityComparer<{string.Join(", ", types.Value)}>.Default");
            }
            else if (attributesMetadata.ReferenceEquality is not null && property.HasAttribute(attributesMetadata.ReferenceEquality))
            {
                sb.Append($"global::Brimborium.BaseMethods.ReferenceEqualityComparer<{typeName}>.Default");
            }
            else if (attributesMetadata.SetEquality is not null && property.HasAttribute(attributesMetadata.SetEquality))
            {
                var types = property.GetIEnumerableTypeArguments()!;
                sb.Append(
                    $"global::Brimborium.BaseMethods.SetEqualityComparer<{string.Join(", ", types.Value)}>.Default");
            }
            else if (attributesMetadata.CustomEquality is not null && property.HasAttribute(attributesMetadata.CustomEquality))
            {
                var attribute = property.GetAttribute(attributesMetadata.CustomEquality);
                var comparerType = (INamedTypeSymbol) attribute?.ConstructorArguments[0].Value!;
                var comparerMemberName = (string) attribute?.ConstructorArguments[1].Value!;

                if (comparerType.GetMembers().Any(x => x.Name == comparerMemberName && x.IsStatic) || comparerType.GetProperties().Any(x => x.Name == comparerMemberName && x.IsStatic))
                {
                    sb.Append($"{comparerType.ToFQF()}.{comparerMemberName}");
                }
                else
                {
                    sb.Append($"new {comparerType.ToFQF()}()");
                }
            }
            else
            {
                sb.Append($"global::System.Collections.Generic.EqualityComparer<{typeName}>.Default");
            }

            sb.AppendLine(");");
        }
    }
}