namespace Brimborium.SourceGenerator.BaseMethods;

internal class RecordStructEqualityGenerator : EqualityGeneratorBase {
    static void BuildEquals(
        EquatableInformationType equatableInformationType,
        StringBuilder sb,
        int level
        ) {
        var symbol = equatableInformationType.TypeSymbol;
        var symbolName = symbol.ToFQF();

        sb.AppendLine(level, InheritDocComment);
        sb.AppendLine(level, GeneratedCodeAttributeDeclaration);
        sb.AppendLine(level, $"public bool Equals({symbolName} other)");
        sb.AppendOpenBracket(ref level);

        sb.AppendLine(level, "return true");
        level++;

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
        int level
        ) {
        sb.AppendLine(level, InheritDocComment);
        sb.AppendLine(level, GeneratedCodeAttributeDeclaration);
        sb.AppendLine(level, @"public override int GetHashCode()");
        sb.AppendOpenBracket(ref level);
        sb.AppendLine(level, @"var hashCode = new global::System.HashCode();");
        sb.AppendLine(level);

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
            includeSelf: true,
            content: (sb, level) => {
                BuildEquals(equatableInformationType, sb, level);

                sb.AppendLine(level);

                BuildGetHashCode(equatableInformationType, sb, level);
            });
    }
}