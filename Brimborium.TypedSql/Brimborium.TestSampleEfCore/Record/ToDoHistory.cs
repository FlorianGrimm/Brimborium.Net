using System;
using System.Collections.Generic;

namespace Brimborium.TestSampleEfCore.Record
{
    public partial class ToDoHistory
    {
        public Guid ActivityId { get; set; }
        public Guid Id { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? UserId { get; set; }
        public string Title { get; set; } = null!;
        public bool Done { get; set; }
        public DateTimeOffset ValidFrom { get; set; }
        public DateTimeOffset ValidTo { get; set; }
        public byte[] SerialVersion { get; set; } = null!;

        public virtual Activity Activity { get; set; } = null!;
    }
}
