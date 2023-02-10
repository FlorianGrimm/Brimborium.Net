using System.Collections.Generic;

namespace TestEffect.Records;

public partial class UnorderedEquality
{
    [Equatable]
    public partial record Sample
    {
        [UnorderedEquality] public List<int>? Properties { get; init; }
    }
}