using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data
{
    public partial class SCSHelpDeskContext : DbContext
    {
        public SCSHelpDeskContext()
        {
        }

        public SCSHelpDeskContext(DbContextOptions<SCSHelpDeskContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Priority> Priorities { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<Ticket> Ticket { get; set; }
        public virtual DbSet<UserPreferences> UserPreferences { get; set; } // Add DbSet for UserPreferences

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Data for Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Super Admin" },
                new Role { RoleId = 2, RoleName = "Admin" },
                new Role { RoleId = 3, RoleName = "Support Agent" },
                new Role { RoleId = 4, RoleName = "Student" }
            );

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            // Seed Data for Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryType = "Enrollment" },
                new Category { CategoryId = 2, CategoryType = "Grades" },
                new Category { CategoryId = 3, CategoryType = "Organizational" },
                new Category { CategoryId = 4, CategoryType = "Inquiry" },
                new Category { CategoryId = 5, CategoryType = "Miscellaneous" }
            );

            // Seed Data for Priorities
            modelBuilder.Entity<Priority>().HasData(
                new Priority { PriorityId = 1, PriorityType = "High" },
                new Priority { PriorityId = 2, PriorityType = "Medium" },
                new Priority { PriorityId = 3, PriorityType = "Low" },
                new Priority { PriorityId = 4, PriorityType = "General" }
            );

            // Seed Data for Statuses
            modelBuilder.Entity<Status>().HasData(
                new Status { StatusId = 1, StatusType = "Open" },
                new Status { StatusId = 2, StatusType = "In Progress" },
                new Status { StatusId = 3, StatusType = "Resolved" },
                new Status { StatusId = 4, StatusType = "Closed" },
                new Status { StatusId = 5, StatusType = "Archived" }
            );

            // Configure UserPreferences Table
            modelBuilder.Entity<UserPreferences>(entity =>
            {
                entity.ToTable("UserPreferences"); // Define table name

                entity.HasKey(up => up.PreferenceId); // Primary key

                entity.Property(up => up.CreatedTime)
                    .IsRequired();

                entity.Property(up => up.UpdatedTime)
                    .IsRequired();

                // Define relationships
                entity.HasOne(up => up.User)
                    .WithOne(u => u.UserPreferences) // One-to-one relationship
                    .HasForeignKey<UserPreferences>(up => up.UserId) // Foreign key in UserPreferences
                    .OnDelete(DeleteBehavior.Cascade); // Cascade on delete

                entity.HasOne(up => up.DefaultCategory)
                    .WithMany()
                    .HasForeignKey(up => up.DefaultCategoryId)
                    .OnDelete(DeleteBehavior.Restrict); // Restrict delete for lookup tables

                entity.HasOne(up => up.DefaultStatus)
                    .WithMany()
                    .HasForeignKey(up => up.DefaultStatusId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(up => up.DefaultPriority)
                    .WithMany()
                    .HasForeignKey(up => up.DefaultPriorityId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
