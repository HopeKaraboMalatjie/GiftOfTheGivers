using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGivers.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ReliefProject> ReliefProjects { get; set; } = null!;
        public DbSet<VolunteerSignup> VolunteerSignups { get; set; } = null!;
        public DbSet<Donation> Donations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relief Project -> Employee (poster) - optional FK, restrict delete
            builder.Entity<ReliefProject>()
                .HasOne(rp => rp.PostedByUser)
                .WithMany()
                .HasForeignKey(rp => rp.PostedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // VolunteerSignup -> ReliefProject
            builder.Entity<VolunteerSignup>()
                .HasOne(v => v.ReliefProject)
                .WithMany(rp => rp.VolunteerSignups)
                .HasForeignKey(v => v.ReliefProjectId)
                .OnDelete(DeleteBehavior.SetNull);

            // VolunteerSignup -> ApplicationUser
            builder.Entity<VolunteerSignup>()
                .HasOne(v => v.ApplicationUser)
                .WithMany()
                .HasForeignKey(v => v.ApplicationUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Donation -> ReliefProject
            builder.Entity<Donation>()
                .HasOne(d => d.ReliefProject)
                .WithMany(rp => rp.Donations)
                .HasForeignKey(d => d.ReliefProjectId)
                .OnDelete(DeleteBehavior.SetNull);

            // Donation -> ApplicationUser
            builder.Entity<Donation>()
                .HasOne(d => d.ApplicationUser)
                .WithMany()
                .HasForeignKey(d => d.ApplicationUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes to support common lookups (see 2.3 Optimising Performance)
            builder.Entity<Donation>().HasIndex(d => d.DonatedOn);
            builder.Entity<Donation>().HasIndex(d => d.ReliefProjectId);
            builder.Entity<VolunteerSignup>().HasIndex(v => v.Email);
            builder.Entity<ReliefProject>().HasIndex(rp => rp.Status);
        }
    }
}
