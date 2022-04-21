using System;
using System.Collections.Generic;

namespace Brimborium.TestSampleEfCore.Record
{
    public partial class Activity
    {
        public Activity()
        {
            Project = new HashSet<Project>();
            ProjectHistory = new HashSet<ProjectHistory>();
            ToDo = new HashSet<ToDo>();
            ToDoHistory = new HashSet<ToDoHistory>();
            User = new HashSet<User>();
            UserHistory = new HashSet<UserHistory>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string EntityType { get; set; } = null!;
        public string EntityId { get; set; } = null!;
        public string? Data { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public byte[] SerialVersion { get; set; } = null!;

        public virtual ICollection<Project> Project { get; set; }
        public virtual ICollection<ProjectHistory> ProjectHistory { get; set; }
        public virtual ICollection<ToDo> ToDo { get; set; }
        public virtual ICollection<ToDoHistory> ToDoHistory { get; set; }
        public virtual ICollection<User> User { get; set; }
        public virtual ICollection<UserHistory> UserHistory { get; set; }
    }
}
