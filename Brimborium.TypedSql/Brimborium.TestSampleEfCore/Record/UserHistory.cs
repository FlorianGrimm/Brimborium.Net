using System;
using System.Collections.Generic;

namespace Brimborium.TestSampleEfCore.Record
{
    public partial class UserHistory
    {
        public Guid ActivityId { get; set; }
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public DateTimeOffset ValidFrom { get; set; }
        public DateTimeOffset ValidTo { get; set; }
        public byte[] SerialVersion { get; set; } = null!;

        public virtual Activity Activity { get; set; } = null!;
    }
}
