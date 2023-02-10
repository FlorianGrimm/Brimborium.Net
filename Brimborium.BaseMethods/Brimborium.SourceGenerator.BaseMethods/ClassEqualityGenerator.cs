namespace Brimborium.SourceGenerator.BaseMethods;

internal class ClassEqualityGenerator : EqualityGeneratorBase {
    static void BuildEquals(
        EquatableInformationType equatableInformationType,
        StringBuilder sb,
        int level) {
        var symbol = equatableInformationType.TypeSymbol;
        var symbolName = symbol.ToFQF();
        var baseTypeName = symbol.BaseType?.ToFQF();

        sb.AppendLine(level, EqualsOperatorCodeComment);
        sb.AppendLine(level, GeneratedCodeAttributeDeclaration);
        sb.AppendLine(level, $"public static bool operator ==({symbolName}? left, {symbolName}? right)");
        sb.AppendLine(level + 1, $" => global::Brimborium.BaseMethods.DefaultEqualityComparer<{symbolName}>.GetDefault().Equals(left, right);");
        sb.AppendLine(level);

        sb.AppendLine(level, NotEqualsOperatorCodeComment);
        sb.AppendLine(level, GeneratedCodeAttributeDeclaration);
        sb.AppendLine(level, $"public static bool operator !=({symbolName}? left, {symbolName}? right)");
        sb.AppendLine(level + 1, $" => !global::Brimborium.BaseMethods.DefaultEqualityComparer<{symbolName}>.GetDefault().Equals(left, right);");
        sb.AppendLine(level);

        sb.AppendLine(level, InheritDocComment);
        sb.AppendLine(level, GeneratedCodeAttributeDeclaration);
        sb.AppendLine(level, "public override bool Equals(object? obj) =>");
        sb.AppendLine(level + 1, $"Equals(obj as {symbolName});");
        sb.AppendLine(level);

        sb.AppendLine(level, InheritDocComment);
        sb.AppendLine(level, GeneratedCodeAttributeDeclaration);
        sb.AppendLine(level, $"public bool Equals({symbolName}? other)");
        sb.AppendOpenBracket(ref level);

        sb.AppendLine(level, "return");
        level++;

        sb.AppendLine(level, baseTypeName == "object" || equatableInformationType.IgnoreInheritedMembers
            ? "!ReferenceEquals(other, null) && this.GetType() == other.GetType()"
            : $"base.Equals(other as {baseTypeName})");

        foreach (var property in equatableInformationType.DictProperty.Values) {
            BuildPropertyEquality(equatableInformationType, property, sb, level);
        }

        sb.AppendLine(level, ";");
        level--;

        sb.AppendCloseBracket(ref level);
    }

    static void BuildGetHashCode(
        EquatableInformationType equatableInformationType,
        StringBuilder sb,
        int level) {
        var symbol = equatableInformationType.TypeSymbol;

        var baseTypeName = symbol.BaseType?.ToFQF();

        sb.AppendLine(level, InheritDocComment);
        sb.AppendLine(level, GeneratedCodeAttributeDeclaration);
        sb.AppendLine(level, @"public override int GetHashCode()");
        sb.AppendOpenBracket(ref level);

        sb.AppendLine(level, @"var hashCode = new global::System.HashCode();");
        sb.AppendLine(level);

        sb.AppendLine(level, baseTypeName == "object" || equatableInformationType.IgnoreInheritedMembers
            ? "hashCode.Add(this.GetType());"
            : "hashCode.Add(base.GetHashCode());");

        foreach (var property in equatableInformationType.DictProperty.Values) {
            BuildPropertyHashCode(equatableInformationType, property, sb, level);
        }

        sb.AppendLine(level);
        sb.AppendLine(level, "return hashCode.ToHashCode();");

        sb.AppendCloseBracket(ref level);
    }

    public static void Generate(
       EquatableInformationType equatableInformationType,
       StringBuilder sb
       ) {
        var symbol = equatableInformationType.TypeSymbol;
        ContainingTypesBuilder.Build(
            sb,
            symbol,
            includeSelf: false,
            content: (sb, level) => {
                var typeName = symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                sb.AppendLine(level, $"partial class {typeName} : global::System.IEquatable<{typeName}>");
                sb.AppendOpenBracket(ref level);

                BuildEquals(equatableInformationType, sb, level);

                sb.AppendLine(level);

                BuildGetHashCode(equatableInformationType, sb, level);

                sb.AppendCloseBracket(ref level);
            });
    }
}