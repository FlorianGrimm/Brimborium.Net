using System.Collections.Generic;

namespace TestEffect.Classes;

public partial class UnorderedEquality
{
    [Equatable]
    public partial class Sample
    {
        [UnorderedEquality] public List<int>? Properties { get; set; }
    }
}