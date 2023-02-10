using System;

namespace TestEffect.RecordStructs;

public partial class ObsoleteRecord
{
    [Equatable]
    [Obsolete("Make sure the obsolete on the object model does not add warnings")]
    public partial record struct Sample(string Name);
}