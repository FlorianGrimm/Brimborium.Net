namespace Brimborium.SourceGenerator.CustomizeTemplate;

[Generator]
public class CustomizeTemplateGenerator : ISourceGenerator {

    public void Initialize(GeneratorInitializationContext context) {
        //if (Debugger.IsAttached) {
        //    // Debugger.Break();
        //} else { 
        //    Debugger.Launch();
        //}
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context) {
        if (context.SyntaxReceiver is not SyntaxReceiver syntaxReceiver) { return; }
        
        // context.Compilation.GetTypeByMetadataName("Brimborium.BaseMethods.CustomEqualityAttribute");
        
    }
    
    class SyntaxReceiver : ISyntaxReceiver {
        readonly List<SyntaxNode> _candidateSyntaxes = new List<SyntaxNode>();

        public IReadOnlyList<SyntaxNode> CandidateSyntaxes => this._candidateSyntaxes;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            if (syntaxNode is ClassDeclarationSyntax 
                || syntaxNode is RecordDeclarationSyntax 
                || syntaxNode is StructDeclarationSyntax) {
                this._candidateSyntaxes.Add(syntaxNode);
            }
            syntaxNode.HasStructuredTrivia
        }
    }
}