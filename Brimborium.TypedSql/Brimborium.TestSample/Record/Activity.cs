
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace Brimborium.TestSample.Record {
    public partial class Activity {
        public Activity() {
            Project = new HashSet<Project>();
            ProjectHistory = new HashSet<ProjectHistory>();
            ToDo = new HashSet<ToDo>();
            ToDoHistory = new HashSet<ToDoHistory>();
            User = new HashSet<User>();
            UserHistory = new HashSet<UserHistory>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(100)]
        public string Title { get; set; } = null!;
        [StringLength(100)]
        public string EntityType { get; set; } = null!;
        [StringLength(100)]
        public string EntityId { get; set; } = null!;
        public string? Data { get; set; }
        [Key]
        public DateTimeOffset CreatedAt { get; set; }
        public ulong SerialVersion { get; set; }

        [InverseProperty("Activity")]
        public virtual ICollection<Project> Project { get; set; }
        [InverseProperty("Activity")]
        public virtual ICollection<ProjectHistory> ProjectHistory { get; set; }
        [InverseProperty("Activity")]
        public virtual ICollection<ToDo> ToDo { get; set; }
        [InverseProperty("Activity")]
        public virtual ICollection<ToDoHistory> ToDoHistory { get; set; }
        [InverseProperty("Activity")]
        public virtual ICollection<User> User { get; set; }
        [InverseProperty("Activity")]
        public virtual ICollection<UserHistory> UserHistory { get; set; }
    }
}
