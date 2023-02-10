namespace Brimborium.SourceGenerator.BaseMethods;

public partial class BaseMethodsGenerator {
    public class Emitter {
        public Emitter() {
        }

        public void Emit(
            StringBuilder sb,
            EquatableInformationType equatableInformation
            , CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            //var typeName = equatableInformation.TypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            
            var typeDeclarationSyntax = equatableInformation.TypeDeclarationSyntax;
            if (typeDeclarationSyntax is StructDeclarationSyntax structDeclarationSyntax) {
                StructEqualityGenerator.Generate(equatableInformation, sb);
            } else if (typeDeclarationSyntax is ClassDeclarationSyntax classDeclarationSyntax) {
                ClassEqualityGenerator.Generate(equatableInformation, sb);
            } else if (typeDeclarationSyntax is RecordDeclarationSyntax recordDeclarationSyntax) {
                //var isClass = recordDeclarationSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.ClassKeyword));
                var isStruct = recordDeclarationSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.StructKeyword));
                if (isStruct) {
                    RecordStructEqualityGenerator.Generate(equatableInformation, sb);
                } else {
                    RecordClassEqualityGenerator.Generate(equatableInformation, sb);
                }
            } else {
                // how??
            }
        }
    }
}