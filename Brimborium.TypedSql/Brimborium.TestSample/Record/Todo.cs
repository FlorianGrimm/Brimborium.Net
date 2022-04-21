
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace Brimborium.TestSample.Record {
    public partial class ToDo {
        [Key]
        public Guid Id { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? UserId { get; set; }
        [StringLength(50)]
        public string Title { get; set; } = null!;
        public bool Done { get; set; }
        public Guid? ActivityId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public long SerialVersion { get; set; }

        [ForeignKey("ModifiedAt,ActivityId")]
        [InverseProperty("ToDo")]
        public virtual Activity? Activity { get; set; }
        [ForeignKey("ProjectId")]
        [InverseProperty("ToDo")]
        public virtual Project? Project { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("ToDo")]
        public virtual User? User { get; set; }
    }
}
