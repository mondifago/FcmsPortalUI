using FcmsPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace FcmsPortalUI.Data
{
    public class FcmsPortalUIContext : DbContext
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
        public DbSet<DailyAttendanceLogEntry> DailyAttendanceLogEntries { get; set; }
        public DbSet<ArchivedStudentPayment> ArchivedStudentPayments { get; set; }
        public DbSet<ArchivedPaymentDetail> ArchivedPaymentDetails { get; set; }

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

            // Configure many-to-many for LearningPath StudentsWithAccess
            modelBuilder.Entity<LearningPath>()
                .HasMany(lp => lp.StudentsWithAccess)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "LearningPathStudentsWithAccess",
                    l => l.HasOne<Student>().WithMany().HasForeignKey("StudentsWithAccessId"),
                    r => r.HasOne<LearningPath>().WithMany().HasForeignKey("LearningPathId"),
                    j =>
                    {
                        j.HasKey("StudentsWithAccessId", "LearningPathId");
                        j.ToTable("LearningPathStudentsWithAccess");
                    });

            modelBuilder.Entity<ClassSession>()
                .HasOne(cs => cs.Teacher)
                .WithMany(t => t.ClassSessions)
                .HasForeignKey(cs => cs.TeacherId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ArchivedPaymentDetail>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Amount)
                    .HasPrecision(10, 2);

                // Foreign Key Relationship
                entity.HasOne(e => e.ArchivedStudentPayment)
                    .WithMany(asp => asp.PaymentDetails)
                    .HasForeignKey(e => e.ArchivedStudentPaymentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.ArchivedStudentPaymentId)
                    .HasDatabaseName("IX_ArchivedPaymentDetail_ArchivedStudentPaymentId");
            });


            modelBuilder.Entity<ArchivedStudentPayment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.StudentName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.LearningPathName)
                    .HasMaxLength(300)
                    .IsRequired();

                entity.Property(e => e.AcademicYear)
                    .HasMaxLength(20)
                    .IsRequired();

                // Indexes for efficient querying
                entity.HasIndex(e => new { e.AcademicYear, e.EducationLevel, e.ClassLevel, e.Semester })
                    .HasDatabaseName("IX_ArchivedStudentPayment_LearningPathFilter");

                entity.HasIndex(e => e.StudentId)
                    .HasDatabaseName("IX_ArchivedStudentPayment_OriginalStudentId");

                entity.HasIndex(e => e.ArchivedDate)
                    .HasDatabaseName("IX_ArchivedStudentPayment_ArchivedDate");
            });


        }
    }
}