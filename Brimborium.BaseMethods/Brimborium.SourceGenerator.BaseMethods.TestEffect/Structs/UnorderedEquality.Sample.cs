using System.Collections.Generic;

namespace TestEffect.Structs;

public partial class UnorderedEquality
{
    [Equatable]
    public partial struct Sample
    {
        [UnorderedEquality] public List<int>? Properties { get; set; }
    }
}