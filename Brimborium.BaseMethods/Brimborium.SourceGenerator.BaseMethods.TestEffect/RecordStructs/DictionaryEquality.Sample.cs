using System.Collections.Generic;

namespace TestEffect.RecordStructs;

public partial class DictionaryEquality
{
    [Equatable]
    public partial record struct Sample
    {
        [UnorderedEquality] public Dictionary<string, int>? Properties { get; init; }
    }
}