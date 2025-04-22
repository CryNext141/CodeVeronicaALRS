using ALRS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ALRS.Data
{
    public class ApplicationDbContext : IdentityDbContext<Users>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Alert>()
                .HasOne(a => a.Victim)
                .WithOne(v => v.Alert)
                .HasForeignKey<Victim>(v => v.AlertId);

            modelBuilder.Entity<Alert>()
                .HasOne(a => a.Abductor)
                .WithOne(ab => ab.Alert)
                .HasForeignKey<Abductor>(ab => ab.AlertId);

            modelBuilder.Entity<Alert>()
                .Property(a => a.CrimeDate)
                .HasColumnType("date");

            modelBuilder.Entity<Alert>()
                .Property(a => a.CrimeTime)
                .HasColumnType("time");

            modelBuilder.Entity<CitizenReport>()
                .Property(r => r.ReportDate)
                .HasColumnType("date");

            modelBuilder.Entity<CitizenReport>()
                .Property(r => r.ReportTime)
                .HasColumnType("time");

            modelBuilder.Entity<Gender>().HasData(
               new Gender { GenderId = 1, Code = "M", DisplayName = "Male" },
               new Gender { GenderId = 2, Code = "F", DisplayName = "Female" },
               new Gender { GenderId = 3, Code = "U", DisplayName = "Unknown" }
            );
            modelBuilder.Entity<SkinColor>().HasData(
                new SkinColor { SkinColorId = 1, Name = "Light" },
                new SkinColor { SkinColorId = 2, Name = "Medium" },
                new SkinColor { SkinColorId = 3, Name = "Dark" },
                new SkinColor { SkinColorId = 4, Name = "Unknown" }
            );

            modelBuilder.Entity<AlertArchive>()
                .Property(a => a.AlertId)
                .ValueGeneratedNever();

            modelBuilder.Entity<AlertStatus>().HasData(
            new AlertStatus { AlertStatusId = 1, Code = "ACTIVE", DisplayName = "Active" },
            new AlertStatus { AlertStatusId = 2, Code = "CLOSED", DisplayName = "Closed" },
            new AlertStatus { AlertStatusId = 3, Code = "CANCELLED", DisplayName = "Cancelled" }
            );

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Alert> Alert { get; set; }
        public DbSet<Victim> Victim { get; set; }
        public DbSet<Abductor> Abductor { get; set; }
        public DbSet<CitizenReport> CitizenReport { get; set; }
        public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<SkinColor> SkinColors { get; set; }
        public DbSet<AlertArchive> AlertArchive { get; set; }
    }
}