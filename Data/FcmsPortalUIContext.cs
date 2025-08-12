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
        public DbSet<Address> Addresses { get; set; }
        public DbSet<LearningPath> LearningPaths { get; set; }
        public DbSet<ClassSession> ClassSessions { get; set; }
        public DbSet<FileAttachment> FileAttachments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SchoolFees> SchoolFees { get; set; }
    }
}