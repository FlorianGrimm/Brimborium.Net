namespace Brimborium.CodeGeneration.CodeAnalysis;

public class ProjectData {
    private List<FileData> _FileDatas;
    private Dictionary<SyntaxTree, Dictionary<string, string>> _DictSyntaxTreeFlags;
    public ProjectData(Project project) {
        this._FileDatas = new List<FileData>();
        this._DictSyntaxTreeFlags = new Dictionary<SyntaxTree, Dictionary<string, string>>();
        this.Project = project;
    }

    public Project Project { get; }
    public ProjectId Id => this.Project.Id;

    public async Task Scan(CancellationToken cancellationToken = default) {
        var compilation = await this.Project.GetCompilationAsync();
        if (compilation is not null) {
            var metadataLoadContext = new MetadataLoadContext(compilation);
            var assemblyWrapper = new AssemblyWrapper(compilation.Assembly, metadataLoadContext);
            var classCollectorAutoGenerate = new ClassCollector();
            var classCollectorNormal = new ClassCollector();
            var _DictSyntaxTreeFlags = new Dictionary<SyntaxTree, Dictionary<string, string>>();
            foreach (var syntaxTree in compilation.SyntaxTrees) {
                Dictionary<string, string>? flags = null;
                var filePath = syntaxTree.FilePath;
                var root = await syntaxTree.GetRootAsync();
                if (root.HasLeadingTrivia) {
                    foreach (var trivia in root.GetLeadingTrivia()) {
                        if (trivia.IsKind(SyntaxKind.MultiLineCommentTrivia)) {
                            var content = trivia.ToString();
                            flags = ReplacementBindingExtension.ReadFlags(content, flags);
                        }
                    }
                }
                if (flags is not null) {
                    _DictSyntaxTreeFlags[syntaxTree] = flags;
                }
                //if (Brimborium.CodeGeneration.ReplacementBindingExtension.HasFlagValue(flags, "Replacements", "on")) {
                //    classCollectorAutoGenerate.Visit(root);
                //} else {
                //    classCollectorNormal.Visit(root);
                //}
                if (ReplacementBindingExtension.HasFlagValue(flags, "AutoGenerate", "on")) {
                    classCollectorAutoGenerate.Visit(root);
                } else if (ReplacementBindingExtension.HasFlagValue(flags, "Replacements", "on")) {
                    classCollectorAutoGenerate.Visit(root);
                } else {
                    classCollectorNormal.Visit(root);
                }
            }

            foreach (var group in classCollectorAutoGenerate.ClassDeclarations.GroupBy(c => c.SyntaxTree)) {
                var syntaxTree = group.Key;
                var compilationSemanticModel = compilation.GetSemanticModel(syntaxTree);
                var compilationUnitSyntax = (CompilationUnitSyntax)syntaxTree.GetRoot(cancellationToken);
                foreach (var classDeclarationSyntax in group) {
                    var declaredSymbol = compilationSemanticModel.GetDeclaredSymbol(classDeclarationSyntax, cancellationToken);
                    if (declaredSymbol is INamedTypeSymbol classTypeSymbol) {
                        var syntaxTrees = classTypeSymbol.DeclaringSyntaxReferences.Select(syntaxReference => syntaxReference.SyntaxTree).Distinct();
                        var classType = classTypeSymbol.AsType(metadataLoadContext);
                        if (classType is not null) {
                            var constructors = classType.GetConstructors();
                            var properties = classType.GetProperties();
                            foreach (var property in properties) {
                            }
                        }

                        foreach (var member in classDeclarationSyntax.Members) {
                            if (member is ConstructorDeclarationSyntax constructorDeclaration) {
                                //var parameterList = constructorDeclaration.ParameterList;
                            } else if (member is PropertyDeclarationSyntax propertyDeclaration) {
                                var typeSyntax = propertyDeclaration.Type;

                                var identifier = propertyDeclaration.Identifier;
                                //identifier.Text
                            }
                        }
                    }
                }
            }
            foreach (var group in classCollectorAutoGenerate.RecordDeclarations.GroupBy(r => r.SyntaxTree)) {
                var syntaxTree = group.Key;
                var compilationSemanticModel = compilation.GetSemanticModel(syntaxTree);
                var compilationUnitSyntax = (CompilationUnitSyntax)syntaxTree.GetRoot(cancellationToken);
                foreach (var recordDeclarationSyntax in group) {
                    var declaredSymbol = compilationSemanticModel.GetDeclaredSymbol(recordDeclarationSyntax, cancellationToken);
                    if (declaredSymbol is INamedTypeSymbol recordTypeSymbol) {
                        var recordType = recordTypeSymbol.AsType(metadataLoadContext);
                        if (recordType is not null) {
                            var constructors = recordType.GetConstructors();
                            var properties = recordType.GetProperties();
                            foreach (var property in properties) {
                            }
                        }
                    }

                    var parameterList = recordDeclarationSyntax.ParameterList;
                    if (parameterList is not null) {
                        foreach (var parameter in parameterList.Parameters) {
                            var type = parameter.Type;
                            var identifier = parameter.Identifier;
                            if (type is PredefinedTypeSyntax predefinedType) {
                            }
                        }
                    }
                    foreach (var member in recordDeclarationSyntax.Members) {
                        if (member is PropertyDeclarationSyntax propertyDeclaration) {
                            var type = propertyDeclaration.Type;
                            var identifier = propertyDeclaration.Identifier;
                        }
                    }
                }
            }
        }
    }
}
public class FileData {
    public Document Document;
    public SemanticModel SemanticModel;

    public FileData(Document document, SemanticModel semanticModel) {
        this.Document = document;
        this.SemanticModel = semanticModel;
    }
}