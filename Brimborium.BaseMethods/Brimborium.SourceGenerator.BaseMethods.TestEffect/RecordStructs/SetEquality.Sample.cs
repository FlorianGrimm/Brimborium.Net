using System.Collections.Generic;

namespace TestEffect.RecordStructs;

public partial class SetEquality
{
    [Equatable]
    public partial record struct Sample
    {
        [SetEquality] public List<int>? Properties { get; set; }
    }
}