using FcmsPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace FcmsPortalUI.Data
{
    public class FcmsPortalUIContext : DbContext
    {
        public FcmsPortalUIContext(DbContextOptions<FcmsPortalUIContext> options) : base(options) { }

        // Core Entities
        public DbSet<School> Schools { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Guardian> Guardians { get; set; }
        public DbSet<Address> Addresses { get; set; }

        // Learning & Academic
        public DbSet<LearningPath> LearningPaths { get; set; }
        public DbSet<ClassSession> ClassSessions { get; set; }
        public DbSet<ScheduleEntry> ScheduleEntries { get; set; }
        public DbSet<CalendarModel> Calendars { get; set; }
        public DbSet<Homework> Homeworks { get; set; }
        public DbSet<HomeworkSubmission> HomeworkSubmissions { get; set; }
        public DbSet<DiscussionThread> DiscussionThreads { get; set; }
        public DbSet<DiscussionPost> DiscussionPosts { get; set; }
        public DbSet<FileAttachment> FileAttachments { get; set; }


        // Assessment & Grading
        public DbSet<TestGrade> TestGrades { get; set; }
        public DbSet<CourseGrade> CourseGrades { get; set; }
        public DbSet<CourseGradingConfiguration> CourseGradingConfigurations { get; set; }


        // Attendance
        public DbSet<DailyAttendanceLogEntry> DailyAttendanceLogEntries { get; set; }

        // Financial
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SchoolFees> SchoolFees { get; set; }

        // Reporting
        public DbSet<LearningPathGradeReport> LearningPathGradeReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Guardian>().OwnsOne(g => g.Person, person =>
            {
                person.OwnsMany(p => p.Addresses);
            });

            modelBuilder.Entity<Student>().OwnsOne(s => s.Person, person =>
            {
                person.OwnsMany(p => p.Addresses);
            });

            modelBuilder.Entity<Staff>().OwnsOne(s => s.Person, person =>
            {
                person.OwnsMany(p => p.Addresses);
            });

            modelBuilder.Entity<School>().OwnsOne(s => s.Address);
        }
    }
}