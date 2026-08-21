using FcmsPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FcmsPortalUI.Data
{
    public class FcmsPortalUIContext : IdentityDbContext<Person, IdentityRole<int>, int>
    {
        public FcmsPortalUIContext(DbContextOptions<FcmsPortalUIContext> options) : base(options) { }

        public DbSet<School> School { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Guardian> Guardians { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<LearningPath> LearningPaths { get; set; }
        public DbSet<StudentReportCard> StudentReportCards { get; set; }
        public DbSet<ScheduleEntry> ScheduleEntries { get; set; }
        public DbSet<ClassSession> ClassSessions { get; set; }
        public DbSet<DiscussionThread> DiscussionThreads { get; set; }
        public DbSet<FirstPost> FirstPosts { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<FileAttachment> FileAttachments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SchoolFees> SchoolFees { get; set; }
        public DbSet<CourseGrade> CourseGrades { get; set; }
        public DbSet<TestGrade> TestGrades { get; set; }
        public DbSet<HomeworkSubmission> HomeworkSubmissions { get; set; }
        public DbSet<DailyAttendanceLogEntry> DailyAttendanceLogEntries { get; set; }
        public DbSet<ArchivedStudentPayment> ArchivedStudentPayments { get; set; }
        public DbSet<ArchivedPaymentDetail> ArchivedPaymentDetails { get; set; }
        public DbSet<AttendanceArchive> AttendanceArchives { get; set; }
        public DbSet<AccountInvitation> AccountInvitations { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<AcademicPeriod> AcademicPeriods { get; set; }
        public DbSet<ArchivedLearningPathPayment> ArchivedLearningPathPayments { get; set; }
        public DbSet<ArchivedSchoolPaymentSummary> ArchivedSchoolPaymentSummaries { get; set; }
        public DbSet<ArchivedLearningPathGrade> ArchivedLearningPathGrades { get; set; }
        public DbSet<ArchivedStudentGrade> ArchivedStudentGrades { get; set; }
        public DbSet<ArchivedCourseGrade> ArchivedCourseGrades { get; set; }
        public DbSet<ArchivedTestGrade> ArchivedTestGrades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<School>()
               .OwnsOne(s => s.Address);

            modelBuilder.Entity<Person>()
                .OwnsOne(p => p.Address);

            modelBuilder.Entity<DailyAttendanceLogEntry>()
                    .HasOne(d => d.LearningPath)
                    .WithMany(lp => lp.AttendanceLog)
                    .HasForeignKey(d => d.LearningPathId)
                    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DailyAttendanceLogEntry>()
                .HasOne(d => d.Teacher)
                .WithMany()
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure many-to-many relationship for PresentStudents
            modelBuilder.Entity<DailyAttendanceLogEntry>()
                .HasMany(d => d.PresentStudents)
                .WithMany()
                .UsingEntity(j => j.ToTable("DailyAttendancePresentStudents"));

            // Configure many-to-many relationship for AbsentStudents  
            modelBuilder.Entity<DailyAttendanceLogEntry>()
                .HasMany(d => d.AbsentStudents)
                .WithMany()
                .UsingEntity(j => j.ToTable("DailyAttendanceAbsentStudents"));

            modelBuilder.Entity<Student>()
                 .HasOne(s => s.Person)
                 .WithOne()
                 .HasForeignKey<Student>(s => s.PersonId)
                 .OnDelete(DeleteBehavior.Cascade);

            // Configure Student and LearningPath relationship
            modelBuilder.Entity<Student>()
                .HasOne(s => s.LearningPath)
                .WithMany(lp => lp.Students)
                .HasForeignKey(s => s.LearningPathId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClassSession>()
                .HasOne(cs => cs.Teacher)
                .WithMany(t => t.ClassSessions)
                .HasForeignKey(cs => cs.TeacherId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // ---- SchoolFees ----

            modelBuilder.Entity<SchoolFees>()
                .HasOne<Student>()
                .WithMany(student => student.SchoolFees)
                .HasForeignKey(schoolFees => schoolFees.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SchoolFees>()
                .HasOne(schoolFees => schoolFees.LearningPath)
                .WithMany()
                .HasForeignKey(schoolFees => schoolFees.LearningPathId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SchoolFees>()
                .HasIndex(schoolFees => new { schoolFees.StudentId, schoolFees.LearningPathId })
                .IsUnique();

            modelBuilder.Entity<SchoolFees>()
                .Navigation(schoolFees => schoolFees.LearningPath)
                .AutoInclude();

            modelBuilder.Entity<SchoolFees>()
                .Navigation(schoolFees => schoolFees.Adjustments)
                .AutoInclude();

            // ---- Payment ----

            modelBuilder.Entity<Payment>()
                .HasOne(payment => payment.LearningPath)
                .WithMany()
                .HasForeignKey(payment => payment.LearningPathId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasIndex(payment => payment.Reference)
                .IsUnique();
        }
    }
}