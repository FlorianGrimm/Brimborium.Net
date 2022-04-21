
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace Brimborium.TestSample.Record {
    public partial class Activity {
        public Activity() {
            this.Project = new HashSet<Project>();
            this.ProjectHistory = new HashSet<ProjectHistory>();
            this.ToDo = new HashSet<ToDo>();
            this.ToDoHistory = new HashSet<ToDoHistory>();
            this.User = new HashSet<User>();
            this.UserHistory = new HashSet<UserHistory>();
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
        public long SerialVersion { get; set; }

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
