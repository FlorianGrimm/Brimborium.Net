﻿using System;
using System.Collections.Generic;

namespace Brimborium.TestSampleEfCore.Record
{
    public partial class User
    {
        public User()
        {
            ToDo = new HashSet<ToDo>();
        }

        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public Guid? ActivityId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public byte[] SerialVersion { get; set; } = null!;

        public virtual Activity? Activity { get; set; }
        public virtual ICollection<ToDo> ToDo { get; set; }
    }
}
