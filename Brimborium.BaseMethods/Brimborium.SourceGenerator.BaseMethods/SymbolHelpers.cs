namespace Brimborium.SourceGenerator.BaseMethods;

public static class SymbolHelpers {
    public static IEnumerable<IPropertySymbol> GetProperties(this ITypeSymbol symbol) {
        var properties = symbol
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => !x.IsStatic && !x.IsIndexer);

        foreach (var property in properties) {
            yield return property;
        }
    }

    // ReSharper disable once InconsistentNaming
    public static string ToNullableFQF(this ISymbol symbol) =>
        symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithMiscellaneousOptions(
                SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
            )
        );

    // ReSharper disable once InconsistentNaming
    public static string ToFQF(this ISymbol symbol) =>
        symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    //public static SymbolDisplayFormat FQF2Format { get; } =
    //        new SymbolDisplayFormat(
    //            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
    //            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
    //            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
    //            miscellaneousOptions:
    //                SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
    //                SymbolDisplayMiscellaneousOptions.UseSpecialTypes);
    //public static string ToFQF2(this ISymbol symbol) =>
    //    symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    public static AttributeData? GetAttribute(this ISymbol symbol, INamedTypeSymbol attribute) {
        return symbol
            .GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.Equals(attribute, SymbolEqualityComparer.Default) == true);
    }

    public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol attribute) =>
        GetAttribute(symbol, attribute) != null;

    public static INamedTypeSymbol? GetInterface(this IPropertySymbol that, string interfaceFqn) {
        return that.Type.AllInterfaces
            .FirstOrDefault(x => x.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == interfaceFqn);
    }

    public static INamedTypeSymbol? GetInterface(this IFieldSymbol that, string interfaceFqn) {
        return that.Type.AllInterfaces
            .FirstOrDefault(x => x.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == interfaceFqn);
    }

    public static INamedTypeSymbol? GetInterface(this IParameterSymbol that, string interfaceFqn) {
        return that.Type.AllInterfaces
            .FirstOrDefault(x => x.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == interfaceFqn);
    }

    public static ImmutableArray<ITypeSymbol>? GetIEnumerableTypeArguments(this IPropertySymbol that) {
        return that.GetInterface("global::System.Collections.Generic.IEnumerable<T>")?.TypeArguments;
    }

    public static ImmutableArray<ITypeSymbol>? GetIEnumerableTypeArguments(this IFieldSymbol that) {
        return that.GetInterface("global::System.Collections.Generic.IEnumerable<T>")?.TypeArguments;
    }

    public static ImmutableArray<ITypeSymbol>? GetIEnumerableTypeArguments(this IParameterSymbol that) {
        return that.GetInterface("global::System.Collections.Generic.IEnumerable<T>")?.TypeArguments;
    }

    public static ImmutableArray<ITypeSymbol>? GetIDictionaryTypeArguments(this IPropertySymbol that) {
        return that.GetInterface("global::System.Collections.Generic.IDictionary<TKey, TValue>")?.TypeArguments;
    }

    public static ImmutableArray<ITypeSymbol>? GetIDictionaryTypeArguments(this IFieldSymbol that) {
        return that.GetInterface("global::System.Collections.Generic.IDictionary<TKey, TValue>")?.TypeArguments;
    }

    public static ImmutableArray<ITypeSymbol>? GetIDictionaryTypeArguments(this IParameterSymbol that) {
        return that.GetInterface("global::System.Collections.Generic.IDictionary<TKey, TValue>")?.TypeArguments;
    }
}