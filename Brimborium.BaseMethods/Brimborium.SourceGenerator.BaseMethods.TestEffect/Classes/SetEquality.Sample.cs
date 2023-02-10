using System.Collections.Generic;

namespace TestEffect.Classes;

public partial class SetEquality
{
    [Equatable]
    public partial class Sample
    {
        [SetEquality] public List<int>? Properties { get; set; }
    }
}