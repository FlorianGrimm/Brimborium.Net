
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace Brimborium.TestSample.Record {
    [Table("ProjectHistory", Schema = "history")]
    public partial class ProjectHistory {
        [Key]
        public Guid ActivityId { get; set; }
        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string Title { get; set; } = null!;
        [Key]
        public DateTimeOffset ValidFrom { get; set; }
        [Key]
        public DateTimeOffset ValidTo { get; set; }
        public long SerialVersion { get; set; }

        [ForeignKey("ValidFrom,ActivityId")]
        [InverseProperty("ProjectHistory")]
        public virtual Activity Activity { get; set; } = null!;
    }
}
