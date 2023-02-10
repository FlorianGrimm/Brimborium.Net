namespace Brimborium.SourceGenerator.BaseMethods;

public class EquatableInformationType {
    public readonly INamedTypeSymbol TypeSymbol;
    public readonly TypeDeclarationSyntax TypeDeclarationSyntax;
    public readonly SortedDictionary<string, EquatableInformationProperty> DictProperty;

    // EquatableAttribute
    /// <summary>
    /// IEquatable will only be generated for explicitly defined attributes.
    /// </summary>
    public bool Explicit { get; set; }

    /// <summary>
    /// Equality and hash code do not consider members of base classes.
    /// </summary>
    public bool IgnoreInheritedMembers { get; set; }

    public EquatableInformationType(
        TypeDeclarationSyntax typeDeclarationSyntax,
        INamedTypeSymbol typeSymbol
        ) {
        this.TypeDeclarationSyntax = typeDeclarationSyntax;
        this.TypeSymbol = typeSymbol;
        this.DictProperty = new SortedDictionary<string, EquatableInformationProperty>();
    }

    public (EquatableInformationProperty value, bool created) AddProperty(
        ISymbol symbol,
        string name
        ) {
        if (!this.DictProperty.TryGetValue(name, out var result)) {
            result = new EquatableInformationProperty(symbol, name);
            this.DictProperty.Add(name, result);
            return (result, true);
        } else {
            return (result, false);
        }
    }
}

public class EquatableInformationProperty {
    public EquatableInformationProperty(
        ISymbol symbol,
        string name
        ) {
        this.ParameterSymbol = symbol as IParameterSymbol;
        this.PropertySymbol = symbol as IPropertySymbol;
        this.Name = name;
    }

    public IParameterSymbol? ParameterSymbol { get; set; }
    public IPropertySymbol? PropertySymbol { get; set; }

    public readonly string Name;

    public bool IsEqualityContract()
        => (this.Name == "EqualityContract");
    // PropertySymbol.ToFQF() == "EqualityContract"

    public bool IgnoreEquality { get; set; }
    public bool DefaultEquality { get; set; }
    public bool OrderedEquality { get; set; }
    public bool UnorderedEquality { get; set; }
    public bool ReferenceEquality { get; set; }
    public bool SetEquality { get; set; }

    public bool CustomEquality { get; set; }
    // 
    public INamedTypeSymbol? EqualityType { get; set; }
    public string? FieldOrPropertyName { get; set; }

    public bool GetCustomEquality()
        => this.CustomEquality
        && this.EqualityType is not null
        && string.IsNullOrEmpty(this.FieldOrPropertyName)
        ;
}