using System.Collections.Generic;

namespace TestEffect.Structs;

public partial class SetEquality
{
    [Equatable]
    public partial struct Sample
    {
        [SetEquality] public List<int>? Properties { get; set; }
    }
}