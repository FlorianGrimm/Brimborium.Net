using System.Collections.Generic;

namespace TestEffect.Classes;

public partial class DictionaryEquality
{
    [Equatable]
    public partial class Sample
    {
        [UnorderedEquality] public Dictionary<string, int>? Properties { get; set; }
    }
}