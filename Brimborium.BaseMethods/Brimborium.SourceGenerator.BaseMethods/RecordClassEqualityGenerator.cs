namespace Brimborium.SourceGenerator.BaseMethods;

internal class RecordClassEqualityGenerator : EqualityGeneratorBase {
    static void BuildEquals(
        EquatableInformationType equatableInformationType,
        StringBuilder sb,
        int level) {
        var symbol = equatableInformationType.TypeSymbol;
        var symbolName = symbol.ToFQF();
        var baseTypeName = symbol.BaseType?.ToFQF();

        sb.AppendLine(level, InheritDocComment);
        sb.AppendLine(level, GeneratedCodeAttributeDeclaration);
        sb.AppendLine(level, symbol.IsSealed
            ? $"public bool Equals({symbolName}? other)"
            : $"public virtual bool Equals({symbolName}? other)");
        sb.AppendOpenBracket(ref level);

        sb.AppendLine(level, "return");
        level++;

        sb.AppendLine(level, baseTypeName == "object" || equatableInformationType.IgnoreInheritedMembers
            ? "!ReferenceEquals(other, null) && EqualityContract == other.EqualityContract"
            : $"base.Equals(({baseTypeName}?)other)");

        foreach (var property in equatableInformationType.DictProperty.Values) {
            if (property.IsEqualityContract()) {
                continue;
            }

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
        var symbol = equatableInformationType.TypeSymbol;
        var baseTypeName = symbol.BaseType?.ToFQF();

        sb.AppendLine(level, InheritDocComment);
        sb.AppendLine(level, GeneratedCodeAttributeDeclaration);
        sb.AppendLine(level, @"public override int GetHashCode()");
        sb.AppendOpenBracket(ref level);
        sb.AppendLine(level, @"var hashCode = new global::System.HashCode();");
        sb.AppendLine(level);

        sb.AppendLine(level, baseTypeName == "object" || equatableInformationType.IgnoreInheritedMembers
            ? "hashCode.Add(this.EqualityContract);"
            : "hashCode.Add(base.GetHashCode());");

        foreach (var property in equatableInformationType.DictProperty.Values) {
            if (property.IsEqualityContract()) { continue; }

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