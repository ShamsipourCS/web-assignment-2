using web_assignment_2.Models;
using Microsoft.EntityFrameworkCore;
using web_assignment_2.Data;

namespace web_assignment_2.Data;

/// <summary>
/// Database context for the Online Learning Management System
/// </summary>
public class SchoolContext : DbContext
{
    public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
    {
    }

    // DbSet properties for each entity
    public DbSet<Teachers> Teachers { get; set; } = null!;
    public DbSet<Students> Students { get; set; } = null!;
    public DbSet<Courses> Courses { get; set; } = null!;
    public DbSet<Enrollment> Enrollments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Teacher entity
        modelBuilder.Entity<Teachers>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.FullName).IsRequired().HasMaxLength(100);
            entity.Property(t => t.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(t => t.Email).IsUnique();
            entity.Property(t => t.HireDate).IsRequired();
        });

        // Configure Student entity
        modelBuilder.Entity<Students>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.FullName).IsRequired().HasMaxLength(100);
            entity.Property(s => s.BirthDate).IsRequired();
            entity.Property(s => s.IsActive).IsRequired();
        });

        // Configure Course entity
        modelBuilder.Entity<Courses>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Title).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Description).HasMaxLength(1000);
            entity.Property(c => c.StartDate).IsRequired();

            // Configure one-to-many relationship: Teacher -> Courses
            entity.HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Enrollment entity (join table for many-to-many)
        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EnrollDate).IsRequired();
            entity.Property(e => e.Grade).HasColumnType("decimal(5,2)");

            // Configure many-to-many relationship: Student <-> Course via Enrollment
            entity.HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Optional: Create composite index for better query performance
            entity.HasIndex(e => new { e.StudentId, e.CourseId });
        });

        // Optional: Seed some initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Teachers
        modelBuilder.Entity<Teachers>().HasData(
            new Teachers
            {
                Id = 1,
                FullName = "Dr. John Smith",
                Email = "john.smith@school.edu",
                HireDate = new DateTime(2020, 1, 15)
            },
            new Teachers
            {
                Id = 2,
                FullName = "Prof. Sarah Johnson",
                Email = "sarah.johnson@school.edu",
                HireDate = new DateTime(2019, 8, 20)
            }
        );

        // Seed Students
        modelBuilder.Entity<Students>().HasData(
            new Students
            {
                Id = 1,
                FullName = "Alice Williams",
                BirthDate = new DateTime(2000, 5, 10),
                IsActive = true
            },
            new Students
            {
                Id = 2,
                FullName = "Bob Brown",
                BirthDate = new DateTime(1999, 12, 25),
                IsActive = true
            },
            new Students
            {
                Id = 3,
                FullName = "Charlie Davis",
                BirthDate = new DateTime(2001, 3, 18),
                IsActive = true
            }
        );

        // Seed Courses
        modelBuilder.Entity<Courses>().HasData(
            new Courses
            {
                Id = 1,
                Title = "Introduction to Programming",
                Description = "Learn the basics of programming with C#",
                StartDate = new DateTime(2024, 9, 1),
                TeacherId = 1
            },
            new Courses
            {
                Id = 2,
                Title = "Database Design",
                Description = "Master database design principles and SQL",
                StartDate = new DateTime(2024, 9, 1),
                TeacherId = 2
            },
            new Courses
            {
                Id = 3,
                Title = "Web Development",
                Description = "Build modern web applications with ASP.NET",
                StartDate = new DateTime(2024, 10, 1),
                TeacherId = 1
            }
        );

        // Seed Enrollments
        modelBuilder.Entity<Enrollment>().HasData(
            new Enrollment
            {
                Id = 1,
                StudentId = 1,
                CourseId = 1,
                EnrollDate = new DateTime(2024, 8, 25),
                Grade = 85.5m
            },
            new Enrollment
            {
                Id = 2,
                StudentId = 1,
                CourseId = 2,
                EnrollDate = new DateTime(2024, 8, 26),
                Grade = 92.0m
            },
            new Enrollment
            {
                Id = 3,
                StudentId = 2,
                CourseId = 1,
                EnrollDate = new DateTime(2024, 8, 25),
                Grade = 78.0m
            },
            new Enrollment
            {
                Id = 4,
                StudentId = 3,
                CourseId = 2,
                EnrollDate = new DateTime(2024, 8, 26),
                Grade = 88.5m
            },
            new Enrollment
            {
                Id = 5,
                StudentId = 3,
                CourseId = 3,
                EnrollDate = new DateTime(2024, 9, 28),
                Grade = 0m // Not yet graded
            }
        );
    }
}