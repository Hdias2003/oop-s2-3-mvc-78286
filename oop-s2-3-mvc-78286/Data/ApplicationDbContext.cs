using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using College.Domain.Models;

namespace oop_s2_3_mvc_78286.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, Role, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<StaffProfile> StaffProfiles { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Enrolments> Enrolments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentResults> AssignmentResults { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Always call the base first for Identity tables
            base.OnModelCreating(builder);

            builder.Entity<Branch>()
                .Property(b => b.Eircode).HasMaxLength(8);

            builder.Entity<StudentProfile>()
                .HasIndex(s => s.IdentityUserID).IsUnique();

            builder.Entity<StaffProfile>()
                .HasIndex(s => s.IdentityUserID).IsUnique();

            // Many-to-Many: Modules <-> Courses
            builder.Entity<Module>()
                .HasMany(m => m.Courses)
                .WithMany(c => c.Modules) // This property now exists in Course.cs
                .UsingEntity(j => j.ToTable("CourseModules"));

            // Many-to-Many: Modules <-> Staff
            builder.Entity<Module>()
                .HasMany(m => m.StaffProfiles)
                .WithMany(s => s.Modules)
                .UsingEntity(j => j.ToTable("StaffModules"));

            builder.Entity<AssignmentResults>()
                .Property(ar => ar.Result)
                .HasColumnType("decimal(5, 2)");

            builder.Entity<Attendance>()
                .HasIndex(a => new { a.StudentProfileId, a.ModuleId, a.SessionDate })
                .IsUnique();

            builder.Entity<Enrolments>()
                .Property(e => e.Status)
                .HasDefaultValue("Active");
        }
        public DbSet<College.Domain.Models.ExamResults> ExamResults { get; set; } = default!;
        public DbSet<College.Domain.Models.Exams> Exams { get; set; } = default!;
    }
}