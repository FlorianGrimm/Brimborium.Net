// [assembly: InternalsVisibleTo("Brimborium.BaseMethods.SnapshotTests")]

namespace Brimborium.BaseMethods {
    [Generator]
    class BaseMethodsGenerator : ISourceGenerator {
        public void Initialize(GeneratorInitializationContext context) {
            //if (Debugger.IsAttached) {
            //    // Debugger.Break();
            //} else { 
            //    Debugger.Launch();
            //}
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context) {
            if (context.SyntaxReceiver is not SyntaxReceiver syntaxReceiver) return;

            var attributesMetadata = new AttributesMetadata(
                context.Compilation.GetTypeByMetadataName("Brimborium.BaseMethods.EquatableAttribute"),
                context.Compilation.GetTypeByMetadataName("Brimborium.BaseMethods.DefaultEqualityAttribute"),
                context.Compilation.GetTypeByMetadataName("Brimborium.BaseMethods.OrderedEqualityAttribute"),
                context.Compilation.GetTypeByMetadataName("Brimborium.BaseMethods.IgnoreEqualityAttribute"),
                context.Compilation.GetTypeByMetadataName("Brimborium.BaseMethods.UnorderedEqualityAttribute"),
                context.Compilation.GetTypeByMetadataName("Brimborium.BaseMethods.ReferenceEqualityAttribute"),
                context.Compilation.GetTypeByMetadataName("Brimborium.BaseMethods.SetEqualityAttribute"),
                context.Compilation.GetTypeByMetadataName("Brimborium.BaseMethods.CustomEqualityAttribute")
            );

            var handledSymbols = new HashSet<string>();

            foreach (var node in syntaxReceiver.CandidateSyntaxes) {
                var model = context.Compilation.GetSemanticModel(node.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(node, context.CancellationToken) as ITypeSymbol;

                var equatableAttributeData = symbol?.GetAttributes().FirstOrDefault(x =>
                    x.AttributeClass?.Equals(attributesMetadata.Equatable, SymbolEqualityComparer.Default) ==
                    true);

                if (equatableAttributeData == null)
                    continue;

                var symbolDisplayString = symbol!.ToDisplayString();

                if (handledSymbols.Contains(symbolDisplayString))
                    continue;

                handledSymbols.Add(symbolDisplayString);

                var explicitMode = equatableAttributeData.NamedArguments
                    .FirstOrDefault(pair => pair.Key == nameof(EquatableAttribute.Explicit))
                    .Value.Value is true;
                var ignoreInheritedMembers = equatableAttributeData.NamedArguments
                    .FirstOrDefault(pair => pair.Key == nameof(EquatableAttribute.IgnoreInheritedMembers))
                    .Value.Value is true;
                var source = node switch {
                    StructDeclarationSyntax _ => StructEqualityGenerator.Generate(symbol!, attributesMetadata, explicitMode),
                    RecordDeclarationSyntax _ when node.RawKind == 9068 => RecordStructEqualityGenerator.Generate(symbol!, attributesMetadata, explicitMode),
                    RecordDeclarationSyntax _ => RecordEqualityGenerator.Generate(symbol!, attributesMetadata, explicitMode, ignoreInheritedMembers),
                    ClassDeclarationSyntax _ => ClassEqualityGenerator.Generate(symbol!, attributesMetadata, explicitMode, ignoreInheritedMembers),
                    _ => throw new Exception("should not have gotten here.")
                };

                var fileName = $"{EscapeFileName(symbolDisplayString)}.Brimborium.BaseMethods.g.cs"!;
                context.AddSource(fileName, source);
            }

            static string EscapeFileName(string fileName) => new[] { '<', '>', ',' }
                .Aggregate(new StringBuilder(fileName), (s, c) => s.Replace(c, '_')).ToString();
        }

        class SyntaxReceiver : ISyntaxReceiver {
            readonly List<SyntaxNode> _candidateSyntaxes = new List<SyntaxNode>();

            public IReadOnlyList<SyntaxNode> CandidateSyntaxes => this._candidateSyntaxes;

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
                if (syntaxNode is ClassDeclarationSyntax || syntaxNode is RecordDeclarationSyntax || syntaxNode is StructDeclarationSyntax) {
                    this._candidateSyntaxes.Add(syntaxNode);
                }
            }
        }
    }
}