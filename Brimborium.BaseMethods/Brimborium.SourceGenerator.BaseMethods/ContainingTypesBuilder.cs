namespace Brimborium.SourceGenerator.BaseMethods;

internal static class ContainingTypesBuilder {
    static IEnumerable<INamespaceOrTypeSymbol> ContainingNamespaceAndTypes(ISymbol symbol, bool includeSelf) {
        foreach (var item in AllContainingNamespacesAndTypes(symbol, includeSelf)) {
            yield return item;

            if (item.IsNamespace) {
                yield break;
            }
        }
    }

    static IEnumerable<INamespaceOrTypeSymbol> AllContainingNamespacesAndTypes(ISymbol symbol, bool includeSelf) {
        if (includeSelf && symbol is INamespaceOrTypeSymbol self) {
            yield return self;
        }

        while (true) {
            symbol = symbol.ContainingSymbol;

            if (!(symbol is INamespaceOrTypeSymbol namespaceOrTypeSymbol)) {
                yield break;
            }

            yield return namespaceOrTypeSymbol;
        }
    }

    public static void Build(
        StringBuilder sb,
        ISymbol symbol,
        Action<StringBuilder, int> content, 
        bool includeSelf = false
        ) {
        // The test cases use 3000 characters on average, and these are the minimum classes.
        // It is also recommended to select a power of two as the initial value.
        var symbols = ContainingNamespaceAndTypes(symbol, includeSelf).ToList();
        var level = 0;

        for (var idxSymbol = symbols.Count - 1; idxSymbol >= 0; idxSymbol--) {
            var s = symbols[idxSymbol];

            if (s.IsNamespace) {
                sb.AppendLine();
                sb.AppendLine(EqualityGeneratorBase.EnableNullableContext);
                sb.AppendLine(EqualityGeneratorBase.SuppressObsoleteWarningsPragma);
                sb.AppendLine();

                var namespaceName = s.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted));
                sb.AppendLine($"namespace {namespaceName}");
                sb.AppendOpenBracket(ref level);
            } else {
                var typeDeclarationSyntax = s.DeclaringSyntaxReferences
                    .Select(x => x.GetSyntax())
                    .OfType<TypeDeclarationSyntax>()
                    .First();

                var keyword = typeDeclarationSyntax switch {
                    ClassDeclarationSyntax _ => "class",
                    StructDeclarationSyntax _ => "struct",
                    RecordDeclarationSyntax recordDeclarationSyntax =>
                        recordDeclarationSyntax.Modifiers.Any(
                            modifier => modifier.IsKind(SyntaxKind.StructKeyword)) ? "record struct" : "record class",
                    _ => throw new ArgumentOutOfRangeException($"Syntax kind {typeDeclarationSyntax.Kind()} not supported")
                };

                var typeName = s.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                sb.AppendLine(level, $"partial {keyword} {typeName}");
                sb.AppendOpenBracket(ref level);
            }
        }

        content(sb, level);
        symbols.ForEach(_ => sb.AppendCloseBracket(ref level));
    }
}