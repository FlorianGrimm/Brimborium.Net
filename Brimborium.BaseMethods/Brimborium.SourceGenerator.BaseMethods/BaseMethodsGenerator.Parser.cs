namespace Brimborium.SourceGenerator.BaseMethods;

public partial class BaseMethodsGenerator {
    internal const string EquatableAttribute = "Brimborium.BaseMethods.EquatableAttribute";
    internal const string DefaultEqualityAttribute = "Brimborium.BaseMethods.DefaultEqualityAttribute";
    internal const string OrderedEqualityAttribute = "Brimborium.BaseMethods.OrderedEqualityAttribute";
    internal const string IgnoreEqualityAttribute = "Brimborium.BaseMethods.IgnoreEqualityAttribute";
    internal const string UnorderedEqualityAttribute = "Brimborium.BaseMethods.UnorderedEqualityAttribute";
    internal const string ReferenceEqualityAttribute = "Brimborium.BaseMethods.ReferenceEqualityAttribute";
    internal const string SetEqualityAttribute = "Brimborium.BaseMethods.SetEqualityAttribute";
    internal const string CustomEqualityAttribute = "Brimborium.BaseMethods.CustomEqualityAttribute";

    public class Parser {
        private readonly CancellationToken _cancellationToken;
        private readonly Compilation _compilation;
        private readonly Action<Diagnostic> _reportDiagnostic;

        public Parser(Compilation compilation, Action<Diagnostic> reportDiagnostic, CancellationToken cancellationToken) {
            _compilation = compilation;
            _cancellationToken = cancellationToken;
            _reportDiagnostic = reportDiagnostic;
        }

        public IReadOnlyList<EquatableInformationType> GetEquatableInformation(
            ImmutableArray<GeneratorAttributeSyntaxContext> arrGeneratorAttributeSyntaxContext
            ) {

            INamedTypeSymbol? equatableAttribute = this._compilation.GetBestTypeByMetadataName(EquatableAttribute);
            INamedTypeSymbol? defaultEqualityAttribute = this._compilation.GetBestTypeByMetadataName(DefaultEqualityAttribute);
            INamedTypeSymbol? orderedEqualityAttribute = this._compilation.GetBestTypeByMetadataName(OrderedEqualityAttribute);
            INamedTypeSymbol? ignoreEqualityAttribute = this._compilation.GetBestTypeByMetadataName(IgnoreEqualityAttribute);
            INamedTypeSymbol? unorderedEqualityAttribute = this._compilation.GetBestTypeByMetadataName(UnorderedEqualityAttribute);
            INamedTypeSymbol? referenceEqualityAttribute = this._compilation.GetBestTypeByMetadataName(ReferenceEqualityAttribute);
            INamedTypeSymbol? setEqualityAttribute = this._compilation.GetBestTypeByMetadataName(SetEqualityAttribute);
            INamedTypeSymbol? customEqualityAttribute = this._compilation.GetBestTypeByMetadataName(CustomEqualityAttribute);

            if (equatableAttribute is null
                || defaultEqualityAttribute is null
                || orderedEqualityAttribute is null
                || ignoreEqualityAttribute is null
                || unorderedEqualityAttribute is null
                || referenceEqualityAttribute is null
                || setEqualityAttribute is null
                ) {
                // nothing to do if this type isn't available
                return Array.Empty<EquatableInformationType>();
            }

            //var unique = new HashSet<TypeDeclarationSyntax>();
            var dictEquatableInformation = new Dictionary<string, EquatableInformationType>();

            var groupsGeneratorAttributeSyntaxContext = arrGeneratorAttributeSyntaxContext.GroupBy(x => x.SemanticModel);
            foreach (var grpGeneratorAttributeSyntaxContext in groupsGeneratorAttributeSyntaxContext) {
                SemanticModel semanticModel = grpGeneratorAttributeSyntaxContext.Key;
                foreach (var generatorAttributeSyntaxContext in grpGeneratorAttributeSyntaxContext) {
                    if (generatorAttributeSyntaxContext.TargetNode is not TypeDeclarationSyntax typeDeclarationSyntax) { continue; }
                    if (generatorAttributeSyntaxContext.TargetSymbol is not INamedTypeSymbol typeDeclarationSymbol) { continue; }

                    var typeDeclarationDisplayName = typeDeclarationSymbol.ToDisplayString(NullableFlowState.None, SymbolDisplayFormat.FullyQualifiedFormat);
                    if (!dictEquatableInformation.TryGetValue(typeDeclarationDisplayName, out var equatableInformation)) {

                        equatableInformation = new EquatableInformationType(
                            typeDeclarationSyntax,
                            typeDeclarationSymbol
                            );
                        dictEquatableInformation.Add(typeDeclarationDisplayName, equatableInformation);

                        foreach (var attributeData in typeDeclarationSymbol.GetAttributes()) {
                            if (attributeData is null) { continue; }
                            foreach (var namedArgument in attributeData.NamedArguments) {
                                if (namedArgument.Key == nameof(equatableInformation.Explicit)) {
                                    if (namedArgument.Value.Value is bool boolValue) {
                                        equatableInformation.Explicit = boolValue;
                                    }
                                } else if (namedArgument.Key == nameof(equatableInformation.IgnoreInheritedMembers)) {
                                    if (namedArgument.Value.Value is bool boolValue) {
                                        equatableInformation.IgnoreInheritedMembers = boolValue;
                                    }
                                }
                            }
                        }
                    }

                    if (typeDeclarationSyntax is RecordDeclarationSyntax recordDeclarationSyntax) {
                        if (recordDeclarationSyntax.ParameterList is ParameterListSyntax parameterListSyntax) {
                            foreach (var parameterSyntax in parameterListSyntax.Parameters) {

                                var declaredSymbol = semanticModel.GetDeclaredSymbol(parameterSyntax);
                                if (declaredSymbol is IParameterSymbol parameterSymbol) {
                                    AddRecordCtorParameter(
                                        parameterSymbol,
                                        parameterSyntax.Identifier.Text,
                                        parameterSymbol.GetAttributes(),
                                        equatableInformation,
                                        semanticModel);
                                    continue;
                                }
                            }
                        }
                    }

                    foreach (var memberInfo in typeDeclarationSymbol.GetMembers()) {
                        if (memberInfo is null) { continue; }

                        if (memberInfo.Kind == SymbolKind.Property) {
                            if (memberInfo is IPropertySymbol propertySymbol) {
                                if (propertySymbol.IsIndexer) { continue; }
                                if (propertySymbol.IsStatic) { continue; }
                                if (propertySymbol.IsWriteOnly) { continue; }
                                //if (propertySymbol.IsImplicitlyDeclared) { continue; }

                                AddProperty(
                                    propertySymbol,
                                    propertySymbol.Name,
                                    propertySymbol.GetAttributes(),
                                    equatableInformation,
                                    semanticModel);
                                continue;
                            }
                        }
                        if (memberInfo.Kind == SymbolKind.Field) {
                            if (memberInfo is IFieldSymbol fieldSymbol) {
                                if (memberInfo.IsStatic) { continue; }
                                if (memberInfo.IsImplicitlyDeclared) { continue; }

                                AddField(
                                    fieldSymbol,
                                    fieldSymbol.Name,
                                    fieldSymbol.GetAttributes(),
                                    equatableInformation,
                                    semanticModel);
                                continue;
                            }
                        }
                    }
                }
            }
            //
            return new List<EquatableInformationType>(dictEquatableInformation.Values);

            void AddRecordCtorParameter(
                IParameterSymbol symbol,
                string name,
                ImmutableArray<AttributeData> arrAttributes,
                EquatableInformationType equatableInformation,
                SemanticModel sm
                ) {
                var (equatableProperty, _) = equatableInformation.AddMember(symbol, name);
                readAttributes(arrAttributes, defaultEqualityAttribute, orderedEqualityAttribute, ignoreEqualityAttribute, unorderedEqualityAttribute, referenceEqualityAttribute, setEqualityAttribute, equatableProperty);
            }
            void AddProperty(
                IPropertySymbol symbol,
                string name,
                ImmutableArray<AttributeData> arrAttributes,
                EquatableInformationType equatableInformation,
                SemanticModel sm
                ) {
                var (equatableProperty, _) = equatableInformation.AddMember(symbol, name);
                if (equatableProperty.PropertySymbol is null
                    && symbol is IPropertySymbol propertySymbol) {
                    //equatableProperty.PropertySymbol = propertySymbol;
                }
                readAttributes(arrAttributes, defaultEqualityAttribute, orderedEqualityAttribute, ignoreEqualityAttribute, unorderedEqualityAttribute, referenceEqualityAttribute, setEqualityAttribute, equatableProperty);
            }
            void AddField(
                IFieldSymbol symbol,
                string name,
                ImmutableArray<AttributeData> arrAttributes,
                EquatableInformationType equatableInformation,
                SemanticModel sm) {
                var (equatableProperty, _) = equatableInformation.AddMember(symbol, name);
                readAttributes(arrAttributes, defaultEqualityAttribute, orderedEqualityAttribute, ignoreEqualityAttribute, unorderedEqualityAttribute, referenceEqualityAttribute, setEqualityAttribute, equatableProperty);
            }

            void readAttributes(ImmutableArray<AttributeData> arrAttributes, INamedTypeSymbol defaultEqualityAttribute, INamedTypeSymbol orderedEqualityAttribute, INamedTypeSymbol ignoreEqualityAttribute, INamedTypeSymbol unorderedEqualityAttribute, INamedTypeSymbol referenceEqualityAttribute, INamedTypeSymbol setEqualityAttribute, EquatableInformationMember equatableProperty) {
                foreach (var attributeData in arrAttributes) {
                    if (defaultEqualityAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default)) {
                        equatableProperty.DefaultEquality = true;
                    } else if (orderedEqualityAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default)) {
                        equatableProperty.OrderedEquality = true;
                    } else if (ignoreEqualityAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default)) {
                        equatableProperty.IgnoreEquality = true;
                    } else if (unorderedEqualityAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default)) {
                        equatableProperty.UnorderedEquality = true;
                    } else if (referenceEqualityAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default)) {
                        equatableProperty.ReferenceEquality = true;
                    } else if (setEqualityAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default)) {
                        equatableProperty.SetEquality = true;

                        foreach (var namedArgument in attributeData.NamedArguments) {
                            if (namedArgument.Key == nameof(equatableProperty.EqualityType)) {
                                if (namedArgument.Value.Value is INamedTypeSymbol equalityType) {
                                    equatableProperty.EqualityType = equalityType;
                                }
                            } else if (namedArgument.Key == nameof(equatableProperty.FieldOrPropertyName)) {
                                if (namedArgument.Value.Value is string fieldOrPropertyName) {
                                    equatableProperty.FieldOrPropertyName = fieldOrPropertyName;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
