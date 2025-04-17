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

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Alert> Alert { get; set; }
        public DbSet<Victim> Victim { get; set; }
        public DbSet<Abductor> Abductor { get; set; }
        public DbSet<CitizenReport> CitizenReport { get; set; }
        public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

    }
}