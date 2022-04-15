using System;
using System.Collections.Generic;

namespace Brimborium.TestSampleEfCore.Record
{
    public partial class Project
    {
        public Project()
        {
            ToDo = new HashSet<ToDo>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public Guid? ActivityId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public ulong SerialVersion { get; set; }

        public virtual Activity? Activity { get; set; }
        public virtual ICollection<ToDo> ToDo { get; set; }
    }
}
