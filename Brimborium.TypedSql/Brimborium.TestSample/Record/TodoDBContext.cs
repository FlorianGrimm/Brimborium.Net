//using System;
//using System.Collections.Generic;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata;

//namespace Brimborium.TestSample.Record {
//    public partial class TodoDBContext : DbContext {
//        public TodoDBContext() {
//        }

//        public TodoDBContext(DbContextOptions<TodoDBContext> options)
//            : base(options) {
//        }

//        public virtual DbSet<Activity> Activity { get; set; } = null!;
//        public virtual DbSet<Project> Project { get; set; } = null!;
//        public virtual DbSet<ProjectHistory> ProjectHistory { get; set; } = null!;
//        public virtual DbSet<ToDo> ToDo { get; set; } = null!;
//        public virtual DbSet<ToDoHistory> ToDoHistory { get; set; } = null!;
//        public virtual DbSet<User> User { get; set; } = null!;
//        public virtual DbSet<UserHistory> UserHistory { get; set; } = null!;

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
//            if (!optionsBuilder.IsConfigured) {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Data Source=parado.dev.solvin.local;Initial Catalog=TodoDB;Integrated Security=true;TrustServerCertificate=True;");
//            }
//        }

//        protected override void OnModelCreating(ModelBuilder modelBuilder) {
//            modelBuilder.Entity<Activity>(entity => {
//                entity.HasKey(e => new { e.CreatedAt, e.Id });

//                entity.Property(e => e.SerialVersion)
//                    .IsRowVersion()
//                    .IsConcurrencyToken();
//            });

//            modelBuilder.Entity<Project>(entity => {
//                entity.Property(e => e.Id).ValueGeneratedNever();

//                entity.Property(e => e.SerialVersion)
//                    .IsRowVersion()
//                    .IsConcurrencyToken();

//                entity.HasOne(d => d.Activity)
//                    .WithMany(p => p.Project)
//                    .HasForeignKey(d => new { d.ModifiedAt, d.ActivityId })
//                    .HasConstraintName("FK_Project_Activity");
//            });

//            modelBuilder.Entity<ProjectHistory>(entity => {
//                entity.HasKey(e => new { e.ValidTo, e.ValidFrom, e.ActivityId, e.Id })
//                    .HasName("PK_history_ProjectHistory");

//                entity.Property(e => e.SerialVersion)
//                    .IsRowVersion()
//                    .IsConcurrencyToken();

//                entity.HasOne(d => d.Activity)
//                    .WithMany(p => p.ProjectHistory)
//                    .HasForeignKey(d => new { d.ValidFrom, d.ActivityId })
//                    .OnDelete(DeleteBehavior.ClientSetNull)
//                    .HasConstraintName("FK_history_ProjectHistory_dbo_Activity");
//            });

//            modelBuilder.Entity<ToDo>(entity => {
//                entity.Property(e => e.Id).ValueGeneratedNever();

//                entity.Property(e => e.SerialVersion)
//                    .IsRowVersion()
//                    .IsConcurrencyToken();

//                entity.HasOne(d => d.Project)
//                    .WithMany(p => p.ToDo)
//                    .HasForeignKey(d => d.ProjectId)
//                    .HasConstraintName("FK_dbo_ToDo_dbo_Project");

//                entity.HasOne(d => d.User)
//                    .WithMany(p => p.ToDo)
//                    .HasForeignKey(d => d.UserId)
//                    .HasConstraintName("FK_dbo_ToDo_dbo_User");

//                entity.HasOne(d => d.Activity)
//                    .WithMany(p => p.ToDo)
//                    .HasForeignKey(d => new { d.ModifiedAt, d.ActivityId })
//                    .HasConstraintName("FK_dbo_ToDo_dbo_Activity");
//            });

//            modelBuilder.Entity<ToDoHistory>(entity => {
//                entity.HasKey(e => new { e.ValidTo, e.ValidFrom, e.ActivityId, e.Id })
//                    .HasName("PK_history_ToDoistory");

//                entity.Property(e => e.SerialVersion)
//                    .IsRowVersion()
//                    .IsConcurrencyToken();

//                entity.HasOne(d => d.Activity)
//                    .WithMany(p => p.ToDoHistory)
//                    .HasForeignKey(d => new { d.ValidFrom, d.ActivityId })
//                    .OnDelete(DeleteBehavior.ClientSetNull)
//                    .HasConstraintName("FK_history_ToDoHistory_dbo_Activity");
//            });

//            modelBuilder.Entity<User>(entity => {
//                entity.Property(e => e.Id).ValueGeneratedNever();

//                entity.Property(e => e.SerialVersion)
//                    .IsRowVersion()
//                    .IsConcurrencyToken();

//                entity.HasOne(d => d.Activity)
//                    .WithMany(p => p.User)
//                    .HasForeignKey(d => new { d.ModifiedAt, d.ActivityId })
//                    .HasConstraintName("FK_dbo_User_dbo_Activity");
//            });

//            modelBuilder.Entity<UserHistory>(entity => {
//                entity.HasKey(e => new { e.ValidTo, e.ValidFrom, e.ActivityId, e.Id })
//                    .HasName("PK_history_UserHistory");

//                entity.Property(e => e.SerialVersion)
//                    .IsRowVersion()
//                    .IsConcurrencyToken();

//                entity.HasOne(d => d.Activity)
//                    .WithMany(p => p.UserHistory)
//                    .HasForeignKey(d => new { d.ValidFrom, d.ActivityId })
//                    .OnDelete(DeleteBehavior.ClientSetNull)
//                    .HasConstraintName("FK_history_UserHistory_dbo_Activity");
//            });

//            OnModelCreatingPartial(modelBuilder);
//        }

//        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
//    }
//}
