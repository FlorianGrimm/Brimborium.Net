
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace Brimborium.TestSample.Record {
    public partial class Project {
        public Project() {
            this.ToDo = new HashSet<ToDo>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string Title { get; set; } = null!;
        public Guid? ActivityId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public long SerialVersion { get; set; }

        [ForeignKey("ModifiedAt,ActivityId")]
        [InverseProperty("Project")]
        public virtual Activity? Activity { get; set; }
        [InverseProperty("Project")]
        public virtual ICollection<ToDo> ToDo { get; set; }
    }
}
