namespace Brimborium.SourceGenerator.BaseMethods;

public class EquatableInformationType {
    public readonly INamedTypeSymbol TypeSymbol;
    public readonly TypeDeclarationSyntax TypeDeclarationSyntax;
    public readonly SortedDictionary<string, EquatableInformationMember> DictMember;

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
        this.DictMember = new SortedDictionary<string, EquatableInformationMember>();
    }

    public List<EquatableInformationMember> GetEnabledMembers()
        => this.DictMember.Values.Where(member => member.IsEnabled()).ToList();

    public (EquatableInformationMember value, bool created) AddMember(
        ISymbol symbol,
        string name
        ) {
        if (!this.DictMember.TryGetValue(name, out var result)) {
            result = new EquatableInformationMember(this, symbol, name);
            this.DictMember.Add(name, result);
            return (result, true);
        } else {
            return (result, false);
        }
    }
}

public class EquatableInformationMember {
    public EquatableInformationMember(
        EquatableInformationType equatableInformationType,
        ISymbol symbol,
        string name
        ) {
        this.EquatableInformationType = equatableInformationType;
        this.Symbol = symbol;
        this.FieldSymbol = symbol as IFieldSymbol;
        this.ParameterSymbol = symbol as IParameterSymbol;
        this.PropertySymbol = symbol as IPropertySymbol;
        this.Name = name;
    }
    public readonly EquatableInformationType EquatableInformationType;
    public readonly ISymbol Symbol;
    public readonly IFieldSymbol? FieldSymbol;
    public readonly IParameterSymbol? ParameterSymbol;
    public readonly IPropertySymbol? PropertySymbol;

    public ITypeSymbol? Type 
        => (this.PropertySymbol?.Type)
        ?? (this.FieldSymbol?.Type)
        ?? (this.ParameterSymbol?.Type)
        ;

    public ImmutableArray<ITypeSymbol>? GetIDictionaryTypeArguments()
        => (this.PropertySymbol?.GetIDictionaryTypeArguments())
        ?? (this.FieldSymbol?.GetIDictionaryTypeArguments())
        ?? (this.ParameterSymbol?.GetIDictionaryTypeArguments())
        ;

    public ImmutableArray<ITypeSymbol>? GetIEnumerableTypeArguments()
        => (this.PropertySymbol?.GetIEnumerableTypeArguments())
        ?? (this.FieldSymbol?.GetIEnumerableTypeArguments())
        ?? (this.ParameterSymbol?.GetIEnumerableTypeArguments())
        ;


    public NullableAnnotation? NullableAnnotation
        => (this.PropertySymbol?.NullableAnnotation)
        ?? (this.FieldSymbol?.NullableAnnotation)
        ?? (this.PropertySymbol?.NullableAnnotation)
        ;

    public readonly string Name;

    public bool IsEqualityContract()
        => (this.Name == "EqualityContract");
    // PropertySymbol.ToFQF() == "EqualityContract"

    //public bool HasNotExplcitAttribute()
    //    => this.IgnoreEquality
    //    ? false
    //    :  this.DefaultEquality
    //    || this.OrderedEquality
    //    || this.UnorderedEquality
    //    || this.ReferenceEquality
    //    || this.SetEquality
    //    || this.CustomEquality
    //    ;

    public bool HasExplcitAttribute()
        => this.DefaultEquality
        || this.OrderedEquality
        || this.UnorderedEquality
        || this.ReferenceEquality
        || this.SetEquality
        || this.CustomEquality
        ;

    private bool? _IsEnabled;
    public bool IsEnabled() {
        if (this._IsEnabled.HasValue) { return this._IsEnabled.Value; }
        if (this.ParameterSymbol is not null) {
            //return (this._IsEnabled = true).Value;
        }
        if (this.PropertySymbol is not null) {
            if (this.EquatableInformationType.Explicit) {
                return (this._IsEnabled = this.HasExplcitAttribute()).Value;
            } else {
                return (this._IsEnabled = true).Value;
            }
        }
        if (this.FieldSymbol is not null) {
            return (this._IsEnabled = this.HasExplcitAttribute()).Value;
        }
        return false;
    }

    public bool IgnoreEquality { get; set; }
    public bool DefaultEquality { get; set; }
    public bool OrderedEquality { get; set; }
    public bool UnorderedEquality { get; set; }
    public bool ReferenceEquality { get; set; }
    public bool SetEquality { get; set; }

    public bool CustomEquality { get; set; }
    // 
#warning this must be a list    
    public INamedTypeSymbol? EqualityType { get; set; }
    public string? FieldOrPropertyName { get; set; }

    public bool GetCustomEquality()
        => this.CustomEquality
        && this.EqualityType is not null
        && string.IsNullOrEmpty(this.FieldOrPropertyName)
        ;
}