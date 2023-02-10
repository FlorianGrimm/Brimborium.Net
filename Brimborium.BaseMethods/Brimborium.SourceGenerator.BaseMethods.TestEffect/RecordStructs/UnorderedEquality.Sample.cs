using System.Collections.Generic;

namespace TestEffect.RecordStructs;

public partial class UnorderedEquality
{
    [Equatable]
    public partial record struct Sample
    {
        [UnorderedEquality] public List<int>? Properties { get; init; }
    }
}