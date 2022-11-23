
namespace Brimborium.BaseMethods
{
    public class AttributesMetadata
    {
        public INamedTypeSymbol? Equatable { get; }
        public INamedTypeSymbol? DefaultEquality { get; }
        public INamedTypeSymbol? OrderedEquality { get; }
        public INamedTypeSymbol? IgnoreEquality { get; }
        public INamedTypeSymbol? UnorderedEquality { get; }
        public INamedTypeSymbol? ReferenceEquality { get; }
        public INamedTypeSymbol? SetEquality { get; }
        public INamedTypeSymbol? CustomEquality { get; }

        public AttributesMetadata(
            INamedTypeSymbol? equatable,
            INamedTypeSymbol? defaultEquality,
            INamedTypeSymbol? orderedEquality,
            INamedTypeSymbol? ignoreEquality,
            INamedTypeSymbol? unorderedEquality, 
            INamedTypeSymbol? referenceEquality, 
            INamedTypeSymbol? setEquality,
            INamedTypeSymbol? customEquality)
        {
            this.Equatable = equatable;
            this.DefaultEquality = defaultEquality;
            this.OrderedEquality = orderedEquality;
            this.IgnoreEquality = ignoreEquality;
            this.UnorderedEquality = unorderedEquality;
            this.ReferenceEquality = referenceEquality;
            this.SetEquality = setEquality;
            this.CustomEquality = customEquality;
        }
    }
}
