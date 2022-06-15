namespace Brimborium.CodeGeneration.CodeAnalysis;

public class ClassCollector : CSharpSyntaxWalker {
    public readonly List<ClassDeclarationSyntax> ClassDeclarations;
    public readonly List<RecordDeclarationSyntax> RecordDeclarations;

    public ClassCollector() {
        this.ClassDeclarations = new List<ClassDeclarationSyntax>();
        this.RecordDeclarations = new List<RecordDeclarationSyntax>();
    }


    public override void VisitClassDeclaration(ClassDeclarationSyntax node) {
        //base.VisitClassDeclaration(node);
        this.ClassDeclarations.Add(node);
    }

    public override void VisitRecordDeclaration(RecordDeclarationSyntax node) {
        //base.VisitRecordDeclaration(node);
        this.RecordDeclarations.Add(node);
    }
}
