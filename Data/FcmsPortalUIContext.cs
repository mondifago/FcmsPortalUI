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
        public DbSet<Address> Addresses { get; set; }
        public DbSet<LearningPath> LearningPaths { get; set; }
        public DbSet<ClassSession> ClassSessions { get; set; }
        public DbSet<FileAttachment> FileAttachments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SchoolFees> SchoolFees { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<School>().OwnsOne(s => s.Address);

            modelBuilder.Entity<Guardian>()
                .OwnsOne(g => g.Person, person =>
                {
                    person.Ignore(p => p.Addresses);
                });

            modelBuilder.Entity<Student>()
                .OwnsOne(s => s.Person, person =>
                {
                    person.Ignore(p => p.Addresses);
                });

            modelBuilder.Entity<Staff>()
                .OwnsOne(s => s.Person, person =>
                {
                    person.Ignore(p => p.Addresses);
                });

            modelBuilder.Entity<LearningPath>()
                  .HasMany(lp => lp.Students)
                  .WithOne()
                  .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DailyAttendanceLogEntry>()
                .HasMany(d => d.PresentStudents)
                .WithMany()
                .UsingEntity(j => j.ToTable("DailyAttendancePresentStudents"));

            modelBuilder.Entity<DailyAttendanceLogEntry>()
                .HasMany(d => d.AbsentStudents)
                .WithMany()
                .UsingEntity(j => j.ToTable("DailyAttendanceAbsentStudents"));

            modelBuilder.Entity<DiscussionThread>()
                .HasOne(dt => dt.FirstPost)
                .WithOne()
                .HasForeignKey<DiscussionPost>(dp => dp.DiscussionThreadId)
                .IsRequired(false);

            modelBuilder.Entity<DiscussionThread>()
                .HasMany(dt => dt.Replies)
                .WithOne()
                .HasForeignKey(dp => dp.DiscussionThreadId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiscussionPost>()
                 .OwnsOne(dp => dp.Author, person =>
                 {
                     person.Ignore(p => p.Addresses);
                 });
        }

    }
}