using System.Runtime.InteropServices.ComTypes;

namespace Brimborium.SourceGenerator.BaseMethods;

// https://andrewlock.net/creating-a-source-generator-part-1-creating-an-incremental-source-generator/
[Generator(LanguageNames.CSharp)]
public partial class BaseMethodsGenerator : IIncrementalGenerator {
    public BaseMethodsGenerator() {
    }

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        // Add the marker attribute
        // context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
        //     "EnumExtensionsAttribute.g.cs",
        //     SourceText.From(SourceGenerationHelper.Attribute, Encoding.UTF8)));

        var lstTypeWithEquatable = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: EquatableAttribute,
            predicate: (syntaxNode, ct) => {
                if (syntaxNode is TypeDeclarationSyntax typeDeclarationSyntax
                    && typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword)) {
                    return true;
                } else {
                    return false;
                }
            },
            transform: static (ctx, _) => {
                return ctx;
            } 
            )
            //.Where(static m => m is not null)!
            ;

        var compilationAndTypeWithEquatable = context.CompilationProvider.Combine(lstTypeWithEquatable.Collect());
        
        context.RegisterSourceOutput(compilationAndTypeWithEquatable,
            static (context, source) => Execute(source.Left, source.Right, context));
    }

    private static void Execute(
        Compilation compilation, 
        ImmutableArray<GeneratorAttributeSyntaxContext> arrGeneratorAttributeSyntaxContext, 
        SourceProductionContext context) {
        if (arrGeneratorAttributeSyntaxContext.IsDefaultOrEmpty) { return; }

        var p = new Parser(compilation, context.ReportDiagnostic, context.CancellationToken);
        var lstEquatableInformation = p.GetEquatableInformation(arrGeneratorAttributeSyntaxContext);

        if (lstEquatableInformation.Count > 0) {
            var sb = new StringBuilder();
            var e = new Emitter();
            foreach (var equatableInformation in lstEquatableInformation) { 
                e.Emit(
                    sb,
                    equatableInformation,
                    context.CancellationToken);
            }
            context.AddSource(
                "Brimborium.BaseMethods.g.cs",
                sb.ToString());
        }
    }

    //private static char[] replaceChars = new[] { '<', '>', ',', '+', ':' };

    //private static string EscapeFileName(string fileName) 
    //    => replaceChars.Aggregate(
    //        new StringBuilder(fileName),
    //        (s, c) => s.Replace(c, '_')
    //        ).ToString();

}

// C:\github.com\dotnet\runtime\src\libraries\Microsoft.Extensions.Logging.Abstractions\gen\LoggerMessageGenerator.Parser.cs