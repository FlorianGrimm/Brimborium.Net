using System.Collections.Generic;

namespace TestEffect.Records;

public partial class SetEquality
{
    [Equatable]
    public partial record Sample
    {
        [SetEquality] public List<int>? Properties { get; set; }
    }
}