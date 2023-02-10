using System.Collections.Generic;

namespace TestEffect.Records;

public partial class DictionaryEquality
{
    [Equatable]
    public partial record Sample
    {
        [UnorderedEquality] public Dictionary<string, int>? Properties { get; init; }
    }
}