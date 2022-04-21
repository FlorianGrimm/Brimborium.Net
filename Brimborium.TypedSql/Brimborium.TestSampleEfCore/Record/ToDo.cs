using System;
using System.Collections.Generic;

namespace Brimborium.TestSampleEfCore.Record
{
    public partial class ToDo
    {
        public Guid Id { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? UserId { get; set; }
        public string Title { get; set; } = null!;
        public bool Done { get; set; }
        public Guid? ActivityId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public byte[] SerialVersion { get; set; } = null!;

        public virtual Activity? Activity { get; set; }
        public virtual Project? Project { get; set; }
        public virtual User? User { get; set; }
    }
}
